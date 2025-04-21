using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.CreditorDebts;
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Sales;
using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;
using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.ApiService.Services;

namespace DeLong_Desktop.Pages.Reports
{
    public partial class ReportPage : Page
    {
        private readonly IServiceProvider _services;
        private readonly ICreditorDebtService _creditorDebtService;
        private readonly IDebtService _debtService;
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly ICashRegisterService _cashRegisterService;
        private readonly ICashWarehouseService _cashWarehouseService;
        private readonly ITransactionItemService _transactionItemService;
        private readonly IPriceService _priceService;

        private string _currentReportType;

        public ReportPage(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            _creditorDebtService = services.GetRequiredService<ICreditorDebtService>();
            _debtService = services.GetRequiredService<IDebtService>();
            _productService = services.GetRequiredService<IProductService>();
            _saleService = services.GetRequiredService<ISaleService>();
            _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
            _cashWarehouseService = services.GetRequiredService<ICashWarehouseService>();
            _transactionItemService = services.GetRequiredService<ITransactionItemService>();
            _priceService = services.GetRequiredService<IPriceService>();
            LoadSummaryDataAsync();
        }

        private async void LoadSummaryDataAsync()
        {
            try
            {
                var debts = await _creditorDebtService.RetrieveAllAsync();
                Dispatcher.Invoke(() => TotalDebtAmount.Text = $"${debts.Sum(d => d.RemainingAmount):N2}");

                var receivables = await _debtService.RetrieveAllAsync();
                Dispatcher.Invoke(() => TotalReceivableAmount.Text = $"${receivables.Sum(r => r.RemainingAmount):N2}");

                var transactionItems = await _transactionItemService.RetrieveAllAsync();
                Dispatcher.Invoke(() => TotalProductCount.Text = $"{transactionItems.Count()} ta");

                var registers = await _cashRegisterService.RetrieveOpenRegistersAsync();
                Dispatcher.Invoke(() => TotalOverviewAmount.Text = $"${registers.FirstOrDefault()?.UsdBalance ?? 0:N2}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DebtReport_Click(object sender, RoutedEventArgs e)
        {
            _currentReportType = "Debt";
            await GenerateDebtReport();
            ShowReportPanel();
        }

        private async void ReceivableReport_Click(object sender, RoutedEventArgs e)
        {
            _currentReportType = "Receivable";
            await GenerateReceivableReport();
            ShowReportPanel();
        }

        private async void ProductReport_Click(object sender, RoutedEventArgs e)
        {
            _currentReportType = "Product";
            await GenerateProductReport();
            ShowReportPanel();
        }

        private async void OverviewReport_Click(object sender, RoutedEventArgs e)
        {
            _currentReportType = "Overview";
            await GenerateOverviewReport();
            ShowReportPanel();
        }

        private void BackToTiles_Click(object sender, RoutedEventArgs e)
        {
            TilePanel.Visibility = Visibility.Visible;
            ReportPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowReportPanel()
        {
            TilePanel.Visibility = Visibility.Collapsed;
            ReportPanel.Visibility = Visibility.Visible;
        }

        private async Task GenerateDebtReport()
        {
            try
            {
                var debts = await _creditorDebtService.RetrieveAllAsync();
                var doc = new FlowDocument
                {
                    PageWidth = 827,
                    PagePadding = new Thickness(30, 20, 20, 20),
                    ColumnWidth = double.MaxValue,
                    FontFamily = new FontFamily("Arial")
                };

                // Sarlavha
                Paragraph header = new Paragraph(new Run($"Delong firmasining {DateTimeOffset.UtcNow:yyyy-MM-dd} ga qarzdorlik ro‘yxati"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(header);

                // Hisobot sanasi
                Paragraph reportDate = new Paragraph(new Run($"Sana: {DateTimeOffset.UtcNow:dd.MM.yyyy}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 10, 0, 2),
                    TextAlignment = TextAlignment.Left,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(reportDate);

                if (debts == null || !debts.Any())
                {
                    Paragraph noData = new Paragraph(new Run("Qarzlar topilmadi"))
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.Red
                    };
                    doc.Blocks.Add(noData);
                }
                else
                {
                    Table table = new Table
                    {
                        CellSpacing = 5,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.White
                    };

                    table.Columns.Add(new TableColumn { Width = new GridLength(50) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(400) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });

                    TableRowGroup headerGroup = new TableRowGroup();
                    TableRow headerRow = new TableRow
                    {
                        Background = Brushes.LightGray
                    };
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("№")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Taminotchi")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Qarz (USD)")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                    headerGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerGroup);

                    TableRowGroup dataGroup = new TableRowGroup();
                    int index = 1;
                    foreach (var debt in debts)
                    {
                        TableRow row = new TableRow();
                        row.Cells.Add(new TableCell(new Paragraph(new Run(index.ToString())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(debt.SupplierName.ToUpper())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run($"${debt.RemainingAmount:N2}")) { TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                        dataGroup.Rows.Add(row);
                        index++;
                    }
                    table.RowGroups.Add(dataGroup);
                    doc.Blocks.Add(table);

                    Paragraph totalAmount = new Paragraph(new Run($"Jami qarz: ${debts.Sum(d => d.RemainingAmount):N2}"))
                    {
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 80, 0),
                        TextAlignment = TextAlignment.Right,
                        Foreground = Brushes.Black
                    };
                    doc.Blocks.Add(totalAmount);
                }

                Dispatcher.Invoke(() =>
                {
                    ReportViewer.Document = null;
                    ReportDocument = doc;
                    ReportViewer.Document = doc;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GenerateReceivableReport()
        {
            try
            {
                var receivables = await _debtService.RetrieveAllAsync();
                var sales = await _saleService.RetrieveAllAsync();
                var groupedReceivables = receivables
                    .Join(sales, r => r.SaleId, s => s.Id, (r, s) => new { Receivable = r, Sale = s })
                    .GroupBy(x => x.Sale.CustomerName)
                    .Select(g => new { CustomerName = g.Key, TotalRemaining = g.Sum(x => x.Receivable.RemainingAmount) })
                    .ToList();

                var doc = new FlowDocument
                {
                    PageWidth = 827,
                    PagePadding = new Thickness(30, 20, 20, 20),
                    ColumnWidth = double.MaxValue,
                    FontFamily = new FontFamily("Arial")
                };

                // Sarlavha
                Paragraph header = new Paragraph(new Run($"Delong firmasining {DateTimeOffset.UtcNow:yyyy-MM-dd} ga haqdorlik ro‘yxati"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(header);

                // Hisobot sanasi
                Paragraph reportDate = new Paragraph(new Run($"Sana: {DateTimeOffset.UtcNow:dd.MM.yyyy}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 10, 0, 2),
                    TextAlignment = TextAlignment.Left,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(reportDate);

                if (groupedReceivables == null || !groupedReceivables.Any())
                {
                    Paragraph noData = new Paragraph(new Run("Haqdorlik topilmadi"))
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.Red
                    };
                    doc.Blocks.Add(noData);
                }
                else
                {
                    Table table = new Table
                    {
                        CellSpacing = 5,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.White
                    };

                    table.Columns.Add(new TableColumn { Width = new GridLength(50) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(400) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });

                    TableRowGroup headerGroup = new TableRowGroup();
                    TableRow headerRow = new TableRow
                    {
                        Background = Brushes.LightGray
                    };
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("№")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Mijoz")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Nasiya (USD)")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                    headerGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerGroup);

                    TableRowGroup dataGroup = new TableRowGroup();
                    int index = 1;
                    foreach (var item in groupedReceivables)
                    {
                        TableRow row = new TableRow();
                        row.Cells.Add(new TableCell(new Paragraph(new Run(index.ToString())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.CustomerName.ToUpper())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run($"${item.TotalRemaining:N2}")) { TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                        dataGroup.Rows.Add(row);
                        index++;
                    }
                    table.RowGroups.Add(dataGroup);
                    doc.Blocks.Add(table);

                    Paragraph totalAmount = new Paragraph(new Run($"Jami nasiya: ${groupedReceivables.Sum(g => g.TotalRemaining):N2}"))
                    {
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 80, 0),
                        TextAlignment = TextAlignment.Right,
                        Foreground = Brushes.Black
                    };
                    doc.Blocks.Add(totalAmount);
                }

                Dispatcher.Invoke(() =>
                {
                    ReportViewer.Document = null;
                    ReportDocument = doc;
                    ReportViewer.Document = doc;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GenerateProductReport()
        {
            try
            {
                var products = await _productService.RetrieveAllAsync() ?? new List<ProductResultDto>();
                Console.WriteLine($"Mahsulotlar soni: {products.Count()}"); // Log

                var groupedItems = new List<object>();

                // Har bir mahsulot uchun narxlarni olish
                foreach (var product in products)
                {
                    var prices = await _priceService.RetrieveAllAsync(product.Id) ?? new List<PriceResultDto>();
                    Console.WriteLine($"Mahsulot ID: {product.Id}, Narxlar soni: {prices.Count()}"); // Log

                    // Narxlar bo'yicha ma'lumotlarni log qilish
                    foreach (var price in prices)
                    {
                        Console.WriteLine($"Mahsulot ID: {product.Id}, Narx ID: {price.Id}, Quantity: {price.Quantity}, CreatedAt: {price.CreatedAt}"); // Log
                    }

                    // Dastlabki narx yozuvini olib tashlash (agar narxlar 1 tadan ko'p bo'lsa)
                    var filteredPrices = prices.Count() > 1
                        ? prices.OrderBy(p => p.CreatedAt).Skip(1).ToList()
                        : prices.ToList();
                    Console.WriteLine($"Mahsulot ID: {product.Id}, Filtrlashdan keyin narxlar: {filteredPrices.Count}"); // Log

                    // Narxlar bo'yicha guruhlash
                    var productPrices = filteredPrices
                        .GroupBy(p => new { p.ProductId, ProductName = product.Name, p.UnitOfMeasure, p.SellingPrice })
                        .Select(g => new
                        {
                            ProductId = g.Key.ProductId,
                            ProductName = g.Key.ProductName,
                            UnitOfMeasure = g.Key.UnitOfMeasure,
                            PriceProduct = g.Key.SellingPrice, // Tannarxi sifatida SellingPrice ishlatiladi
                            TotalQuantity = g.Sum(p => p.Quantity) // Miqdor PriceService dan olinadi
                        })
                        .Where(g => g.TotalQuantity > 0) // Faqat qoldiqi 0 dan katta bo'lganlarni ko'rsatamiz
                        .ToList();

                    Console.WriteLine($"Mahsulot ID: {product.Id}, Guruhlangan narxlar soni: {productPrices.Count}"); // Log
                    foreach (var item in productPrices)
                    {
                        Console.WriteLine($"Mahsulot: {item.ProductName}, Tannarxi: {item.PriceProduct}, Miqdor: {item.TotalQuantity} (Tip: {item.TotalQuantity.GetType()})"); // Log
                    }

                    groupedItems.AddRange(productPrices);
                }

                // Mahsulot nomlari bo'yicha tartiblash
                groupedItems = groupedItems.OrderBy(g => (g as dynamic).ProductName).ToList();
                Console.WriteLine($"Umumiy guruhlangan elementlar soni: {groupedItems.Count}"); // Log

                var doc = new FlowDocument
                {
                    PageWidth = 827,
                    PagePadding = new Thickness(30, 20, 20, 20),
                    ColumnWidth = double.MaxValue,
                    FontFamily = new FontFamily("Arial")
                };

                // Sarlavha
                Paragraph header = new Paragraph(new Run($"Delong firmasining {DateTimeOffset.UtcNow:yyyy-MM-dd} ga mahsulotlar ro‘yxati"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(header);

                // Hisobot sanasi
                Paragraph reportDate = new Paragraph(new Run($"Sana: {DateTimeOffset.UtcNow:dd.MM.yyyy}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 10, 0, 2),
                    TextAlignment = TextAlignment.Left,
                    Foreground = Brushes.Black
                };
                doc.Blocks.Add(reportDate);

                if (!groupedItems.Any())
                {
                    Paragraph noData = new Paragraph(new Run("Mahsulotlar topilmadi"))
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.Red
                    };
                    doc.Blocks.Add(noData);
                }
                else
                {
                    Table table = new Table
                    {
                        CellSpacing = 5,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.White
                    };

                    table.Columns.Add(new TableColumn { Width = new GridLength(50) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(200) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(100) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(100) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(120) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });

                    TableRowGroup headerGroup = new TableRowGroup();
                    TableRow headerRow = new TableRow
                    {
                        Background = Brushes.LightGray
                    };
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("№")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Mahsulot")) { FontWeight = FontWeights.Bold, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Miqdor")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("O‘lchov")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Tannarxi")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Jami")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                    headerGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerGroup);

                    TableRowGroup dataGroup = new TableRowGroup();
                    int index = 1;
                    foreach (var item in groupedItems)
                    {
                        var productItem = (dynamic)item;
                        decimal totalQuantity = productItem.TotalQuantity; // decimal sifatida olamiz
                        decimal priceProduct = productItem.PriceProduct; // decimal sifatida olamiz
                        TableRow row = new TableRow();
                        row.Cells.Add(new TableCell(new Paragraph(new Run(index.ToString())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(productItem.ProductName.ToUpper())) { Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(totalQuantity.ToString("N2"))) { TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(productItem.UnitOfMeasure)) { TextAlignment = TextAlignment.Center, Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run($"${priceProduct:N2}")) { TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run($"${(totalQuantity * priceProduct):N2}")) { TextAlignment = TextAlignment.Right, Foreground = Brushes.Black }));
                        dataGroup.Rows.Add(row);
                        index++;
                    }
                    table.RowGroups.Add(dataGroup);
                    doc.Blocks.Add(table);

                    // Jami summa
                    decimal totalSum = groupedItems
                        .Cast<dynamic>()
                        .Sum(t => (decimal)t.TotalQuantity * (decimal)t.PriceProduct); // decimal sifatida Sum qilamiz
                    Paragraph totalAmount = new Paragraph(new Run($"Jami summa: ${totalSum:N2}"))
                    {
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 80, 0),
                        TextAlignment = TextAlignment.Right,
                        Foreground = Brushes.Black
                    };
                    doc.Blocks.Add(totalAmount);
                }

                Dispatcher.Invoke(() =>
                {
                    ReportViewer.Document = null;
                    ReportDocument = doc;
                    ReportViewer.Document = doc;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Xatolik tafsilotlari: {ex.StackTrace}"); // Log
            }
        }
        private async Task GenerateOverviewReport()
        {
            try
            {
                var debts = await _creditorDebtService.RetrieveAllAsync();
                var receivables = await _debtService.RetrieveAllAsync();
                var registers = await _cashRegisterService.RetrieveOpenRegistersAsync();
                var warehouse = await _cashWarehouseService.RetrieveByIdAsync();
                var transactionItems = await _transactionItemService.RetrieveAllAsync();

                var doc = CreateFlowDocument("Delong firmasining " + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd") + " ga umumiy holati");

                doc.Blocks.Add(new Paragraph(new Run($"Jami qarzdorlik: ${debts.Sum(d => d.RemainingAmount):N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                });
                doc.Blocks.Add(new Paragraph(new Run($"Jami haqdorlik: ${receivables.Sum(r => r.RemainingAmount):N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                });

                var cashRegister = registers.FirstOrDefault();
                if (cashRegister != null)
                {
                    doc.Blocks.Add(new Paragraph(new Run("Kassa qoldiqlari:")) { FontSize = 14, FontWeight = FontWeights.Bold });
                    doc.Blocks.Add(new Paragraph(new Run($"Naqd (UZS): {cashRegister.UzsBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                    doc.Blocks.Add(new Paragraph(new Run($"Plastik (UZP): {cashRegister.UzpBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                    doc.Blocks.Add(new Paragraph(new Run($"Dollar (USD): {cashRegister.UsdBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                }

                if (warehouse != null)
                {
                    doc.Blocks.Add(new Paragraph(new Run("Zaxira qoldiqlari:")) { FontSize = 14, FontWeight = FontWeights.Bold });
                    doc.Blocks.Add(new Paragraph(new Run($"Naqd (UZS): {warehouse.UzsBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                    doc.Blocks.Add(new Paragraph(new Run($"Plastik (UZP): {warehouse.UzpBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                    doc.Blocks.Add(new Paragraph(new Run($"Dollar (USD): {warehouse.UsdBalance:N2}"))
                    {
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 80, 0),
                        TextAlignment = TextAlignment.Right
                    });
                }

                doc.Blocks.Add(new Paragraph(new Run($"Mahsulotlar jami summasi: ${transactionItems.Sum(t => t.Quantity * t.PriceProduct):N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 80, 0),
                    TextAlignment = TextAlignment.Right
                });

                Dispatcher.Invoke(() => ReportDocument = doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateFlowDocument(string title)
        {
            var doc = new FlowDocument
            {
                PageWidth = 827,
                PagePadding = new Thickness(30, 20, 20, 20),
                ColumnWidth = double.MaxValue,
                FontFamily = new FontFamily("Arial")
            };
            doc.Blocks.Add(new Paragraph(new Run(title))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Black
            });
            return doc;
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)ReportDocument).DocumentPaginator, "Hisobot");
                    MessageBox.Show("Hisobot chop etildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chop etishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}