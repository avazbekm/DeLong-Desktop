using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using DeLong_Desktop.ApiService.Interfaces;

namespace DeLong_Desktop.Pages.InputHistories;

public partial class DetailsWindow : Window
{
    private readonly HistoryItem _historyItem;
    private readonly ICreditorDebtService _creditorDebtService;
    private readonly ICreditorDebtPaymentService _creditorDebtPaymentService;
    private readonly ITransactionItemService _transactionItemService;
    private readonly IUserService _userService;
    private List<ProductItem> _products;
    private List<PaymentItem> _payments;
    private string _receiverName;

    public DetailsWindow(HistoryItem item, ICreditorDebtService creditorDebtService,
        ICreditorDebtPaymentService creditorDebtPaymentService, ITransactionItemService transactionItemService, IUserService userService)
    {
        InitializeComponent();
        _historyItem = item;
        _creditorDebtService = creditorDebtService;
        _creditorDebtPaymentService = creditorDebtPaymentService;
        _transactionItemService = transactionItemService;
        _userService = userService;

        LoadDetailsAsync();
    }

    private async void LoadDetailsAsync()
    {
        try
        {
            // Qabul qilgan xodim ismini olish
            var debtDetails = await _creditorDebtService.RetrieveByIdAsync(_historyItem.Id);
            var userFullName = await _userService.RetrieveByIdAsync(debtDetails.CreatedBy.Value);

            _receiverName = $"{char.ToUpper(userFullName.FirstName[0])}{userFullName.FirstName.Substring(1)} " +
                $"{char.ToUpper(userFullName.LastName[0])}{userFullName.LastName.Substring(1)}";
            // Mahsulotlar ro‘yxatini olish
            var transactionItems = await _transactionItemService.RetrieveAllAsync();
            _products = transactionItems
                .Where(t => t.TransactionId == _historyItem.Id)
                .Select(t => new ProductItem
                {
                    Id = t.Id,
                    Name = t.ProductName,
                    Quantity = t.Quantity,
                    UnitOfMeasure = t.UnitOfMeasure,
                    PriceProduct = t.PriceProduct,
                    Total = t.Quantity * t.PriceProduct
                })
                .ToList();

            // To‘lovlar ro‘yxatini olish
            _payments = (await _creditorDebtPaymentService.RetrieveAllAsync())
                .Where(p => p.CreditorDebtId == _historyItem.Id)
                .Select(p => new PaymentItem
                {
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount
                })
                .ToList();

            // FlowDocument ni yaratish va ko‘rsatish
            scrollViewer.Document = CreatePrintDocument();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tafsilotlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private FlowDocument CreatePrintDocument()
    {
        FlowDocument doc = new FlowDocument
        {
            PageWidth = 827,
            PagePadding = new Thickness(30, 20, 20, 20),
            ColumnWidth = double.MaxValue,
            FontFamily = new FontFamily("Arial")
        };

        // Sarlavha
        Paragraph header = new Paragraph(new Run("Yuk Tafsilotlari"))
        {
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center
        };
        doc.Blocks.Add(header);

        // Yuk ID
        Paragraph yukId = new Paragraph(new Run($"Yuk ID: {_historyItem.Id}"))
        {
            FontSize = 14,
            Margin = new Thickness(0, 10, 0, 2),
            TextAlignment = TextAlignment.Left
        };
        doc.Blocks.Add(yukId);

        // Taminotchi va sana
        Paragraph supplierInfo = new Paragraph
        {
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 0),
            TextAlignment = TextAlignment.Left
        };
        supplierInfo.Inlines.Add(new Run($"Taminotchi: {_historyItem.SupplierName.ToUpper()}"));
        supplierInfo.Inlines.Add(new Run(new string(' ', 80)));
        supplierInfo.Inlines.Add(new Run($"Sana: {_historyItem.Date:dd.MM.yyyy}"));
        doc.Blocks.Add(supplierInfo);

        // Xodim
        Paragraph receiverInfo = new Paragraph(new Run($"Qabul qilgan xodim: {_receiverName}"))
        {
            FontSize = 14,
            Margin = new Thickness(0, 5, 0, 10),
            TextAlignment = TextAlignment.Left
        };
        doc.Blocks.Add(receiverInfo);

        // Mahsulotlar jadvali
        Table table = new Table
        {
            CellSpacing = 5,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black
        };

        table.Columns.Add(new TableColumn { Width = new GridLength(50) }); // ID
        table.Columns.Add(new TableColumn { Width = new GridLength(200) }); // Mahsulot
        table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // Miqdor
        table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // Birlik
        table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // Tannarxi
        table.Columns.Add(new TableColumn { Width = new GridLength(150) }); // Jami

        TableRowGroup headerGroup = new TableRowGroup();
        TableRow headerRow = new TableRow();
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ID")) { FontWeight = FontWeights.Bold }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Mahsulot")) { FontWeight = FontWeights.Bold }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Miqdor")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Birlik")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Tannarxi")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Jami")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right }));
        headerGroup.Rows.Add(headerRow);
        table.RowGroups.Add(headerGroup);

        TableRowGroup dataGroup = new TableRowGroup();
        foreach (var item in _products)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Id.ToString()))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Name))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.UnitOfMeasure)) { TextAlignment = TextAlignment.Center }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.PriceProduct.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(item.Total.ToString("N2"))) { TextAlignment = TextAlignment.Right }));
            dataGroup.Rows.Add(row);
        }
        table.RowGroups.Add(dataGroup);
        doc.Blocks.Add(table);

        // Umumiy summa
        Paragraph totalAmount = new Paragraph(new Run($"Umumiy summa: ${_historyItem.TotalAmount:N2}"))
        {
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 10, 80, 0),
            TextAlignment = TextAlignment.Right
        };
        doc.Blocks.Add(totalAmount);

        // To‘lovlar
        if (_payments.Any())
        {
            foreach (var payment in _payments)
            {
                Paragraph paymentInfo = new Paragraph(new Run($"To‘lov: ${payment.Amount:N2} (Sana: {payment.PaymentDate:dd.MM.yyyy})"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(paymentInfo);
            }
        }

        // Qolgan qarz
        Paragraph debtInfo = new Paragraph(new Run($"Qolgan qarz: ${_historyItem.RemainingAmount:N2}"))
        {
            FontSize = 14,
            Margin = new Thickness(0, 5, 80, 0),
            TextAlignment = TextAlignment.Right
        };
        doc.Blocks.Add(debtInfo);

        return doc;
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        PrintDialog printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            try
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)scrollViewer.Document).DocumentPaginator, "Yuk Tafsilotlari");
                MessageBox.Show("Tafsilotlar chop etildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chop etishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}

public class ProductItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; }
    public decimal PriceProduct { get; set; }
    public decimal Total { get; set; }
}

public class PaymentItem
{
    public DateTimeOffset PaymentDate { get; set; }
    public decimal Amount { get; set; }
}