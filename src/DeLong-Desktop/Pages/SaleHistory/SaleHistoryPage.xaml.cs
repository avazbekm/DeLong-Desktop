using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Sales;
using DeLong_Desktop.ApiService.DTOs.Payments;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Discounts;
using DeLong_Desktop.ApiService.Helpers;

namespace DeLong_Desktop.Pages.SaleHistory;

public partial class SaleHistoryPage : Page
{
    private readonly ISaleService _saleService;
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;
    private readonly ISaleItemService _saleItemService;
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;
    private readonly IDebtService _debtService;
    private readonly IDiscountService _discountService;
    private List<SaleDisplayItem> allSaleItems;
    private string CustomerName;

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
        LoadSales();
    }

    private async void LoadSales()
    {
        try
        {
            var sales = await _saleService.RetrieveAllAsync();
            allSaleItems = new List<SaleDisplayItem>();

            if (sales != null && sales.Any())
            {
                // Oxirgi sotuv birinchi qatorda turishi uchun CreatedAt bo‘yicha teskari tartiblash
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
                        CreatedAt = TimeHelper.ConvertToUzbekistanTime( sale.CreatedAt)
                    });
                }
                saleDataGrid.ItemsSource = allSaleItems;
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
        string searchText = txtSearch.Text.ToLower();

        var filteredItems = allSaleItems.Where(item =>
        {
            bool matchesCustomer = item.CustomerName.ToLower().Contains(searchText);
            bool matchesAmount = item.TotalAmount.ToString().Contains(searchText);
            return matchesCustomer || matchesAmount;
        }).ToList();

        saleDataGrid.ItemsSource = filteredItems;
    }

    private async void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        if (saleDataGrid.SelectedItem is SaleDisplayItem selectedSale)
        {
            try
            {
                // printga berish uchun customer nameni olindi
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

                PrintSaleItems(printItems, sale, payments, debts, discounts, selectedSale.CreatedAt);
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

    private void PrintSaleItems(List<SaleItemPrintModel> saleItems, SaleResultDto sale,
        IEnumerable<PaymentResultDto> payments, IEnumerable<DebtResultDto> debts, IEnumerable<DiscountResultDto> discounts, DateTimeOffset createdAt)
    {
        PrintDialog printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnWidth = double.MaxValue;
            doc.FontFamily = new FontFamily("Arial");

            Paragraph header = new Paragraph(new Run("Sotuv Cheki"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            // chekId qo'shamiz
            Paragraph chekId = new Paragraph(new Run($"ChekId: {sale.Id}"))
            {
                FontSize = 14,
                Margin = new Thickness(0, 10, 0, 2),
                TextAlignment = TextAlignment.Left
            };
            doc.Blocks.Add(chekId);

            // Mijoz va sana bir qatorda
            Paragraph customerInfo = new Paragraph();
            customerInfo.FontSize = 14;
            customerInfo.Margin = new Thickness(0, 0, 0, 0);
            customerInfo.TextAlignment = TextAlignment.Left; // Umumiy qatorni chapga joylashtiramiz

            // Mijoz nomi (chapda)
            Run customerNameRun = new Run($"Mijoz: {CustomerName.ToUpper()}");
            customerInfo.Inlines.Add(customerNameRun);

            // Bo‘sh joy qo‘shish (o‘rtada bo‘sh joy uchun)
            customerInfo.Inlines.Add(new Run(new string(' ', 100)));

            // Sana (o‘ngda)
            Run dateRun = new Run($"Sana: {createdAt:dd-MM-yyyy}");
            customerInfo.Inlines.Add(dateRun);

            doc.Blocks.Add(customerInfo);

            Table table = new Table();
            table.CellSpacing = 5;
            table.BorderThickness = new Thickness(1);
            table.BorderBrush = Brushes.Black;

            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
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

            Paragraph totalAmount = new Paragraph(new Run($"Jami: {sale.TotalAmount:N2}"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 55, 0),
                TextAlignment = TextAlignment.Right
            };
            doc.Blocks.Add(totalAmount);

            // To‘lovlar – "Debt" turi chiqarilmaydi
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
                        Margin = new Thickness(0, 5, 55, 0),
                        TextAlignment = TextAlignment.Right
                    };
                    doc.Blocks.Add(paymentInfo);
                }
            }

            // Qarzlar – faqat bitta qarz ko‘rsatiladi
            if (debts != null && debts.Any())
            {
                var latestDebt = debts.OrderByDescending(d => d.Id).First();
                Paragraph debtInfo = new Paragraph(new Run($"Qarz: {latestDebt.RemainingAmount:N2} (Muddat: {latestDebt.DueDate:dd-MM-yyyy})"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 55, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(debtInfo);
            }

            // Chegirmalar
            if (discounts != null && discounts.Any())
            {
                foreach (var discount in discounts)
                {
                    Paragraph discountInfo = new Paragraph(new Run($"Chegirma: {discount.Amount:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 55, 0),
                        TextAlignment = TextAlignment.Right
                    };
                    doc.Blocks.Add(discountInfo);
                }
            }

            try
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Sotuv Cheki");
                MessageBox.Show("Chek chop etildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chop etishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}