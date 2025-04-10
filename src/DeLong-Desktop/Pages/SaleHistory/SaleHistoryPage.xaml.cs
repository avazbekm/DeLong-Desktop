using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Threading;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.Pages.SalesPractice;
using DeLong_Desktop.ApiService.DTOs.Sales;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Payments;
using DeLong_Desktop.ApiService.DTOs.Discounts;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.SaleHistory;

public partial class SaleHistoryPage : Page
{
    private readonly ISaleService _saleService;
    private readonly IUserService _userService;
    private readonly IDebtService _debtService;
    private readonly IPaymentService _paymentService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ISaleItemService _saleItemService;
    private readonly IDiscountService _discountService;
    private List<SaleDisplayItem> allSaleItems;
    private string CustomerName;
    private CollectionViewSource collectionViewSource;
    private DispatcherTimer searchTimer;

    public SaleHistoryPage(IServiceProvider services)
    {
        InitializeComponent();
        _saleService = services.GetRequiredService<ISaleService>();
        _userService = services.GetRequiredService<IUserService>();
        _customerService = services.GetRequiredService<ICustomerService>();
        _saleItemService = services.GetRequiredService<ISaleItemService>();
        _productService = services.GetRequiredService<IProductService>();
        _paymentService = services.GetRequiredService<IPaymentService>();
        _debtService = services.GetRequiredService<IDebtService>();
        _discountService = services.GetRequiredService<IDiscountService>();

        allSaleItems = new List<SaleDisplayItem>();
        collectionViewSource = new CollectionViewSource();
        searchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300),
            IsEnabled = false
        };
        searchTimer.Tick += async (s, e) => await FilterSalesAsync();

        this.Loaded += async (s, e) => await LoadSalesAsync();

        if (Application.Current.MainWindow is Window mainWindow)
        {
            var salePracticePage = FindChild<SalePracticePage>(mainWindow);
            if (salePracticePage != null)
            {
                salePracticePage.SaleCompleted += (s, e) => Refresh();
            }
        }
    }

    public async void Refresh()
    {
        await LoadSalesAsync();
    }

    private async Task LoadSalesAsync()
    {
        try
        {
            var sales = await _saleService.RetrieveAllAsync();
            allSaleItems.Clear();

            if (sales != null && sales.Any())
            {
                foreach (var sale in sales.OrderByDescending(s => s.CreatedAt))
                {
                    string customerName = "Noma'lum";
                    if (sale.CustomerId.HasValue)
                    {
                        var customer = await _customerService.RetrieveByIdAsync(sale.CustomerId.Value);
                        customerName = customer != null ? $"{customer.Name}" : "Noma'lum";
                    }
                    else if (sale.UserId.HasValue)
                    {
                        var user = await _userService.RetrieveByIdAsync(sale.UserId.Value);
                        customerName = user != null ? $"{user.FirstName} {user.LastName}" : "Noma'lum";
                    }

                    allSaleItems.Add(new SaleDisplayItem
                    {
                        Id = sale.Id,
                        CustomerName = customerName.ToUpper(),
                        TotalAmount = sale.TotalAmount,
                        CreatedAt = TimeHelper.ConvertToUzbekistanTime(sale.CreatedAt)
                    });
                }

                collectionViewSource.Source = allSaleItems;
                saleDataGrid.ItemsSource = collectionViewSource.View;
            }
            else
            {
                MessageBox.Show("Hozircha sotuvlar mavjud emas!", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Sotuvlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SearchTextChanged(object sender, TextChangedEventArgs e)
    {
        searchTimer.Stop();
        searchTimer.Start();
    }

    private async Task FilterSalesAsync()
    {
        searchTimer.Stop();
        string searchText = txtSearch.Text.ToLower();

        if (collectionViewSource != null)
        {
            collectionViewSource.Filter -= CollectionViewSource_Filter;
            collectionViewSource.Filter += CollectionViewSource_Filter;

            void CollectionViewSource_Filter(object s, FilterEventArgs args)
            {
                var item = args.Item as SaleDisplayItem;
                if (item != null)
                {
                    bool matchesText = string.IsNullOrEmpty(searchText) ||
                                       item.CustomerName.ToLower().Contains(searchText) ||
                                       item.TotalAmount.ToString().Contains(searchText);
                    args.Accepted = matchesText;
                }
            }

            collectionViewSource.View.Refresh();
        }
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        DateTime? startDate = startDatePicker.SelectedDate;
        DateTime? endDate = endDatePicker.SelectedDate;

        if (startDate == null || endDate == null)
        {
            MessageBox.Show("Iltimos, boshlang'ich va tugash sanasini tanlang!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (startDate > endDate)
        {
            MessageBox.Show("Boshlang'ich sana tugash sanasidan katta bo'lmasligi kerak!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (collectionViewSource != null)
        {
            collectionViewSource.Filter -= CollectionViewSource_Filter;
            collectionViewSource.Filter += CollectionViewSource_Filter;

            void CollectionViewSource_Filter(object s, FilterEventArgs args)
            {
                var item = args.Item as SaleDisplayItem;
                if (item != null)
                {
                    string searchText = txtSearch.Text.ToLower();
                    bool matchesText = string.IsNullOrEmpty(searchText) ||
                                       item.CustomerName.ToLower().Contains(searchText) ||
                                       item.TotalAmount.ToString().Contains(searchText);
                    bool matchesDate = item.CreatedAt.Date >= startDate.Value.Date && item.CreatedAt.Date <= endDate.Value.Date;
                    args.Accepted = matchesText && matchesDate;
                }
            }

            collectionViewSource.View.Refresh();
        }
    }

    private async void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        if (saleDataGrid.SelectedItem is SaleDisplayItem selectedSale)
        {
            try
            {
                CustomerName = selectedSale.CustomerName;

                if (sender is Button button)
                {
                    button.IsEnabled = false;
                }

                long saleId = selectedSale.Id;

                var sale = await _saleService.RetrieveByIdAsync(saleId);
                if (sale == null)
                {
                    MessageBox.Show("Sotuv topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var saleItemsToPrint = await _saleItemService.RetrieveAllBySaleIdAsync(saleId);
                if (saleItemsToPrint == null || !saleItemsToPrint.Any())
                {
                    MessageBox.Show("Bu sotuv uchun mahsulotlar topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var payments = await _paymentService.RetrieveBySaleIdAsync(saleId);
                var debts = await _debtService.RetrieveBySaleIdAsync(saleId);
                var discounts = await _discountService.RetrieveBySaleIdAsync(saleId);

                var printItems = new List<SaleItemPrintModel>();
                foreach (var item in saleItemsToPrint)
                {
                    var product = await _productService.RetrieveByIdAsync(item.ProductId);
                    printItems.Add(new SaleItemPrintModel
                    {
                        SerialNumber = item.Id.ToString(),
                        ProductName = product?.Name ?? item.ProductId.ToString(),
                        Price = item.UnitPrice,
                        Quantity = item.Quantity,
                        Unit = item.UnitOfMeasure,
                        TotalPrice = item.UnitPrice * item.Quantity
                    });
                }

                ShowPrintPreview(printItems, sale, payments, debts, discounts, selectedSale.CreatedAt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Pechat qilishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }
        else
        {
            MessageBox.Show("Iltimos, avval ro'yxatdan sotuvni tanlang!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ShowPrintPreview(List<SaleItemPrintModel> saleItems, SaleResultDto sale,
        IEnumerable<PaymentResultDto> payments, IEnumerable<DebtResultDto> debts,
        IEnumerable<DiscountResultDto> discounts, DateTimeOffset createdAt)
    {
        Window previewWindow = new Window
        {
            Title = "Chekni Ko'rish",
            Width = 827,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        FlowDocumentScrollViewer scrollViewer = new FlowDocumentScrollViewer();
        FlowDocument doc = CreatePrintDocument(saleItems, sale, payments, debts, discounts, createdAt);
        scrollViewer.Document = doc;

        StackPanel buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 10, 10)
        };

        Button printButton = new Button
        {
            Content = "Pechat qilish",
            Margin = new Thickness(0, 0, 10, 0),
            Padding = new Thickness(10, 5, 10, 5)
        };

        Button closeButton = new Button
        {
            Content = "Yopish",
            Padding = new Thickness(10, 5, 10, 5)
        };

        printButton.Click += (s, e) =>
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                try
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Sotuv Cheki");
                    MessageBox.Show("Chek chop etildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                    previewWindow.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chop etishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        };

        closeButton.Click += (s, e) => previewWindow.Close();

        buttonPanel.Children.Add(printButton);
        buttonPanel.Children.Add(closeButton);

        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        Grid.SetRow(scrollViewer, 0);
        Grid.SetRow(buttonPanel, 1);

        grid.Children.Add(scrollViewer);
        grid.Children.Add(buttonPanel);

        previewWindow.Content = grid;
        previewWindow.ShowDialog();
    }

    private FlowDocument CreatePrintDocument(List<SaleItemPrintModel> saleItems, SaleResultDto sale,
        IEnumerable<PaymentResultDto> payments, IEnumerable<DebtResultDto> debts,
        IEnumerable<DiscountResultDto> discounts, DateTimeOffset createdAt)
    {
        FlowDocument doc = new FlowDocument
        {
            PageWidth = 827,
            PagePadding = new Thickness(30, 20, 20, 20),
            ColumnWidth = double.MaxValue,
            FontFamily = new FontFamily("Arial")
        };

        Paragraph header = new Paragraph(new Run("Sotuv Cheki"))
        {
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center
        };
        doc.Blocks.Add(header);

        Paragraph chekId = new Paragraph(new Run($"ChekId: {sale.Id}"))
        {
            FontSize = 14,
            Margin = new Thickness(0, 10, 0, 2),
            TextAlignment = TextAlignment.Left
        };
        doc.Blocks.Add(chekId);

        Paragraph customerInfo = new Paragraph
        {
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 0),
            TextAlignment = TextAlignment.Left
        };
        customerInfo.Inlines.Add(new Run($"Mijoz: {CustomerName.ToUpper()}"));
        customerInfo.Inlines.Add(new Run(new string(' ', 100)));
        customerInfo.Inlines.Add(new Run($"Sana: {TimeHelper.ConvertToUzbekistanTime(createdAt):dd.MM.yyyy}"));
        doc.Blocks.Add(customerInfo);

        Table table = new Table
        {
            CellSpacing = 5,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black
        };

        table.Columns.Add(new TableColumn { Width = new GridLength(200) });
        table.Columns.Add(new TableColumn { Width = new GridLength(120) });
        table.Columns.Add(new TableColumn { Width = new GridLength(100) });
        table.Columns.Add(new TableColumn { Width = new GridLength(100) });
        table.Columns.Add(new TableColumn { Width = new GridLength(150) });

        TableRowGroup headerGroup = new TableRowGroup();
        TableRow headerRow = new TableRow();
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Mahsulot")) { FontWeight = FontWeights.Bold }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Narx")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Miqdor")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Birlik")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Jami")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerGroup.Rows.Add(headerRow);
        table.RowGroups.Add(headerGroup);

        TableRowGroup dataGroup = new TableRowGroup();
        foreach (var item in saleItems)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.ProductName))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Price.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Unit)) { TextAlignment = TextAlignment.Center }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.TotalPrice.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            dataGroup.Rows.Add(row);
        }
        table.RowGroups.Add(dataGroup);
        doc.Blocks.Add(table);

        Paragraph totalAmount = new Paragraph(new Run($"Jami: ${sale.TotalAmount:N2}"))
        {
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 10, 80, 0),
            TextAlignment = TextAlignment.Right
        };
        doc.Blocks.Add(totalAmount);

        if (payments != null && payments.Any())
        {
            foreach (var payment in payments)
            {
                if (payment.Type == "Debt") continue;
                string paymentType = payment.Type switch
                {
                    "Cash" => "Naqd",
                    "Card" => "Plastik",
                    "Dollar" => "Dollar",
                    _ => payment.Type
                };
                Paragraph paymentInfo = new Paragraph(new Run($"{paymentType}: {payment.Amount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(paymentInfo);
            }
        }

        if (debts != null && debts.Any())
        {
            var latestDebt = debts.OrderByDescending(d => d.Id).First();
            Paragraph debtInfo = new Paragraph(new Run($"Qarz: ${latestDebt.RemainingAmount:N2} (Muddat: {TimeHelper.ConvertToUzbekistanTime(latestDebt.DueDate):dd.MM.yyyy})"))
            {
                FontSize = 14,
                Margin = new Thickness(0, 5, 80, 0),
                TextAlignment = TextAlignment.Right
            };
            doc.Blocks.Add(debtInfo);
        }

        if (discounts != null && discounts.Any())
        {
            foreach (var discount in discounts)
            {
                Paragraph discountInfo = new Paragraph(new Run($"Chegirma: ${discount.Amount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(discountInfo);
            }
        }

        return doc;
    }

    private T FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;

        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                return typedChild;
            }

            var result = FindChild<T>(child);
            if (result != null) return result;
        }
        return null;
    }
}