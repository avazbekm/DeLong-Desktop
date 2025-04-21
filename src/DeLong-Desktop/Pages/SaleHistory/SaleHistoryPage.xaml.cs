using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.DTOs.Sales;
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Payments;
using DeLong_Desktop.ApiService.DTOs.Discounts;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.Pages.SalesPractice;

namespace DeLong_Desktop.Pages.SaleHistory;

public partial class SaleHistoryPage : Page, INotifyPropertyChanged
{
    private readonly ISaleService _saleService;
    private readonly IUserService _userService;
    private readonly IDebtService _debtService;
    private readonly IPaymentService _paymentService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ISaleItemService _saleItemService;
    private readonly IDiscountService _discountService;
    private ObservableCollection<SaleDisplayItem> _saleItems;
    private string _customerName;
    private CollectionViewSource _collectionViewSource;
    private DispatcherTimer _searchTimer;
    private bool _isLoading;

    public ObservableCollection<SaleDisplayItem> SaleItems
    {
        get => _saleItems;
        set { _saleItems = value; OnPropertyChanged(); }
    }

    public SaleHistoryPage(IServiceProvider services)
    {
        InitializeComponent();
        DataContext = this;

        _saleService = services.GetRequiredService<ISaleService>();
        _userService = services.GetRequiredService<IUserService>();
        _customerService = services.GetRequiredService<ICustomerService>();
        _saleItemService = services.GetRequiredService<ISaleItemService>();
        _productService = services.GetRequiredService<IProductService>();
        _paymentService = services.GetRequiredService<IPaymentService>();
        _debtService = services.GetRequiredService<IDebtService>();
        _discountService = services.GetRequiredService<IDiscountService>();

        SaleItems = new ObservableCollection<SaleDisplayItem>();
        _collectionViewSource = new CollectionViewSource { Source = SaleItems };
        saleDataGrid.ItemsSource = _collectionViewSource.View;

        _searchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300),
            IsEnabled = false
        };
        _searchTimer.Tick += async (s, e) => await FilterSalesAsync();

        // Debug uchun TraceLevel
        System.Diagnostics.PresentationTraceSources.SetTraceLevel(saleDataGrid.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);

        this.Loaded += async (s, e) => await LoadSalesAsync();
        this.Unloaded += (s, e) => Cleanup();

        // SaleCompleted eventi subscribe
        if (Application.Current.MainWindow is Window mainWindow)
        {
            var salePracticePage = FindChild<SalePracticePage>(mainWindow);
            if (salePracticePage != null)
            {
                salePracticePage.SaleCompleted -= SalePracticePage_SaleCompleted;
                salePracticePage.SaleCompleted += SalePracticePage_SaleCompleted;
            }
        }
    }

    private async void SalePracticePage_SaleCompleted(object sender, EventArgs e)
    {
        if (!_isLoading)
        {
            await LoadSalesAsync(); // RefreshAsync o'rniga LoadSalesAsync ishlatamiz
        }
    }

    private void Cleanup()
    {
        _searchTimer?.Stop();
        SaleItems.Clear();
        if (Application.Current.MainWindow is Window mainWindow)
        {
            var salePracticePage = FindChild<SalePracticePage>(mainWindow);
            if (salePracticePage != null)
            {
                salePracticePage.SaleCompleted -= SalePracticePage_SaleCompleted;
            }
        }
    }

    public async Task RefreshAsync()
    {
        if (_isLoading) return;
        await LoadSalesAsync();
    }

    private async Task LoadSalesAsync(int pageSize = 100, int pageNumber = 1)
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            Debug.WriteLine($"LoadSalesAsync started at {DateTime.Now}");
            var stopwatch = Stopwatch.StartNew();

            // Paginatsiya (API da qo‘llab-quvvatlanmasa, local filter)
            var sales = await _saleService.RetrieveAllAsync(); // Agar API paginatsiyani qo‘llab-quvvatlasa, RetrievePagedAsync ishlatiladi
            var pagedSales = sales?.OrderByDescending(s => s.CreatedAt) // Eng yangi sotuv birinchi keladi
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            stopwatch.Stop();
            Debug.WriteLine($"API call took {stopwatch.ElapsedMilliseconds} ms");

            if (pagedSales == null || !pagedSales.Any())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    SaleItems.Clear();
                    MessageBox.Show("Hozircha sotuvlar mavjud emas!", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
                }, DispatcherPriority.Background);
                return;
            }

            // DataGrid ni vaqtincha freeze
            await Dispatcher.InvokeAsync(() => saleDataGrid.IsEnabled = false, DispatcherPriority.Background);

            var itemsToAdd = new List<SaleDisplayItem>();
            foreach (var sale in pagedSales)
            {
                // Agar sotuv allaqachon ro'yxatda bo'lsa, qayta qo'shmaymiz
                if (SaleItems.Any(item => item.Id == sale.Id))
                    continue;

                string customerName = "Noma'lum";
                if (sale.CustomerId.HasValue)
                {
                    var customer = await _customerService.RetrieveByIdAsync(sale.CustomerId.Value);
                    customerName = customer != null ? $"{customer.CompanyName}" : "Noma'lum";
                }
                else if (sale.UserId.HasValue)
                {
                    var user = await _userService.RetrieveByIdAsync(sale.UserId.Value);
                    customerName = user != null ? $"{user.FirstName} {user.LastName}" : "Noma'lum";
                }

                itemsToAdd.Add(new SaleDisplayItem
                {
                    Id = sale.Id,
                    CustomerName = customerName.ToUpper(),
                    TotalAmount = sale.TotalAmount,
                    CreatedAt = TimeHelper.ConvertToUzbekistanTime(sale.CreatedAt)
                });
            }

            // Batch tarzda qo‘shish
            await Dispatcher.InvokeAsync(() =>
            {
                SaleItems.Clear(); // Ro'yxatni tozalaymiz va yangi tartibda qo'shamiz
                foreach (var item in itemsToAdd.OrderByDescending(i => i.CreatedAt)) // Eng yangi birinchi
                {
                    SaleItems.Add(item); // Yangi sotuvlar ro'yxat boshiga qo'shiladi
                }
                saleDataGrid.ItemsSource = _collectionViewSource.View;
                saleDataGrid.IsEnabled = true;
            }, DispatcherPriority.Background);

            Debug.WriteLine($"LoadSalesAsync completed at {DateTime.Now}");
        }
        catch (Exception ex)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show($"Sotuvlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                saleDataGrid.IsEnabled = true;
            }, DispatcherPriority.Background);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task FilterSalesAsync()
    {
        _searchTimer.Stop();
        string searchText = txtSearch.Text.ToLower();

        await Dispatcher.InvokeAsync(() =>
        {
            _collectionViewSource.Filter -= CollectionViewSource_Filter;
            _collectionViewSource.Filter += CollectionViewSource_Filter;

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

            _collectionViewSource.View.Refresh();
        }, DispatcherPriority.Background);
    }

    private void SearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchTimer.Stop();
        _searchTimer.Start();
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

        Dispatcher.Invoke(() =>
        {
            _collectionViewSource.Filter -= CollectionViewSource_Filter;
            _collectionViewSource.Filter += CollectionViewSource_Filter;

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

            _collectionViewSource.View.Refresh();
        });
    }

    private async void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        if (saleDataGrid.SelectedItem is SaleDisplayItem selectedSale)
        {
            try
            {
                _customerName = selectedSale.CustomerName;

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
        customerInfo.Inlines.Add(new Run($"Mijoz: {_customerName.ToUpper()}"));
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}