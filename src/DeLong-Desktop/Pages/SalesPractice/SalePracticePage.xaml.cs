using System.Windows;
using ClosedXML.Excel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using DeLong_Desktop.Pages.Cashs;
using System.Collections.ObjectModel;
using DeLong_Desktop.Windows.DollarKurs;
using DeLong_Desktop.ApiService.Helpers; // DebtEvents uchun qo‘shildi
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Sales;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.Windows.Sales.DebtPacker;
using DeLong_Desktop.ApiService.DTOs.Payments;
using DeLong_Desktop.ApiService.DTOs.SaleItems;
using DeLong_Desktop.ApiService.DTOs.Discounts;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.Windows.Sales.PrintOrExcel;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.Windows.Sales.SelectionCustomer;

namespace DeLong_Desktop.Pages.SalesPractice;

public partial class SalePracticePage : Page
{
    private readonly IUserService _userService;
    private readonly IServiceProvider services;
    private readonly ISaleService _saleService;
    private readonly IDebtService _debtService;
    private readonly IPriceService _priceService;
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;
    private readonly ICustomerService _customerService;
    private readonly IDiscountService _discountService;
    private readonly ISaleItemService _saleItemService;
    private readonly IKursDollarService _kursDollarService;
    private readonly ICashRegisterService _cashRegisterService;
    private readonly ICashTransferService _cashTransferService;

    private TextBox lastUpdatedTextBox;
    private DateTime? _selectedDebtDate = null;
    private long? SelectedCustomerId;

    public ObservableCollection<ProductItem> Items { get; set; } = new();

    public event EventHandler SaleCompleted;
    public static event EventHandler DebtUpdated; // Yangi event qo‘shildi

    public SalePracticePage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _saleService = services.GetRequiredService<ISaleService>();
        _debtService = services.GetRequiredService<IDebtService>();
        _userService = services.GetRequiredService<IUserService>();
        _priceService = services.GetRequiredService<IPriceService>();
        _paymentService = services.GetRequiredService<IPaymentService>();
        _productService = services.GetRequiredService<IProductService>();
        _customerService = services.GetRequiredService<ICustomerService>();
        _saleItemService = services.GetRequiredService<ISaleItemService>();
        _discountService = services.GetRequiredService<IDiscountService>();
        _kursDollarService = services.GetRequiredService<IKursDollarService>();
        _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
        _cashTransferService = services.GetRequiredService<ICashTransferService>();

        ProductGrid.ItemsSource = Items;

        Items.CollectionChanged += (s, e) => UpdateTotalSum();

        LoadingProductData();
        LoadingCustomersData();
        LoadDollarRate();
    }

    private async void LoadDollarRate()
    {
        try
        {
            var latestRate = await _kursDollarService.RetrieveByIdAsync();
            if (latestRate != null && latestRate.TodayDate.Equals(DateTime.Now.ToString("dd.MM.yyyy")))
            {
                tbDolarKurs.Text = latestRate.SellingDollar.ToString("F2");
            }
            else
                MessageBox.Show("Bugungi dollar kursni o'rnating.", "Kurs ma'lumot");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Dollar kursini yuklashda xatolik: {ex.Message}");
        }
    }

    private void UpdateTotalSum()
    {
        decimal totalSum = Items.Sum(item => item.TotalPrice);
        tbTotalPrice.Text = totalSum.ToString("N2");
    }

    private async void LoadingProductData()
    {
        try
        {
            var products = await _productService.RetrieveAllAsync();
            if (products == null || !products.Any())
            {
                return;
            }

            var comboboxItems = products.Select(product => new ComboboxItem
            {
                Id = product.Id,
                ProductName = char.ToUpper(product.Name[0]) + product.Name[1..]
            }).ToList();

            cbxProduct.ItemsSource = comboboxItems;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }
    }

    private List<ComboboxCustomerItem> allCustomers = new();
    private async void LoadingCustomersData()
    {
        try
        {
            var customers = await _customerService.RetrieveAllAsync();
            var users = await _userService.RetrieveAllAsync();

            if (customers != null)
            {
                allCustomers.AddRange(customers.Select(customer => new ComboboxCustomerItem
                {
                    CustomerId = customer.Id,
                    Name = $"{char.ToUpper(customer.Name[0]) + customer.Name[1..]} {customer.Phone}"
                }));
            }

            if (users != null)
            {
                allCustomers.AddRange(users.Select(user => new ComboboxCustomerItem
                {
                    UserId = user.Id,
                    Name = $"{char.ToUpper(user.FirstName[0]) + user.FirstName[1..]}  " +
                           $"{char.ToUpper(user.LastName[0]) + user.LastName[1..]} {user.Phone}"
                }));
            }

            cbxPayment.ItemsSource = allCustomers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }
    }

    private async void btnProductSell_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cbxProduct.SelectedItem is not ComboboxItem selectedProduct ||
                !decimal.TryParse(tbQuantity.Text, out decimal quantity))
            {
                MessageBox.Show("Mahsulotni tanlang yoki miqdorini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var prices = await _priceService.RetrieveAllAsync(selectedProduct.Id);
            if (prices == null || !prices.Any())
            {
                MessageBox.Show("Mahsulot uchun narx topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var price in prices)
            {
                if (price.Quantity >= quantity)
                {
                    var newItem = new ProductItem
                    {
                        PriceId = price.Id,
                        ProductId = price.ProductId,
                        ProductName = selectedProduct.ProductName,
                        SerialNumber = Items.Count + 1,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = quantity,
                        CostPrice = price.CostPrice,
                        BalanceAmount = price.Quantity
                    };

                    // Quantity 0 yoki null bo'lsa qo'shmaslik
                    if (newItem.Quantity <= 0)
                    {
                        continue; // Bu qatorni o'tkazib yuboramiz
                    }

                    Items.Add(newItem);
                    break;
                }
                else if (price.Quantity < quantity)
                {
                    var newItem = new ProductItem
                    {
                        PriceId = price.Id,
                        ProductId = price.ProductId,
                        ProductName = selectedProduct.ProductName,
                        SerialNumber = Items.Count + 1,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = price.Quantity,
                        CostPrice = price.CostPrice,
                        BalanceAmount = price.Quantity
                    };

                    // Quantity 0 yoki null bo'lsa qo'shmaslik
                    if (newItem.Quantity <= 0)
                    {
                        continue; // Bu qatorni o'tkazib yuboramiz
                    }

                    Items.Add(newItem);
                    quantity -= price.Quantity;

                    if (quantity == 0)
                        break;
                }
            }

            // Quantity 0 yoki null bo'lgan qatorlarni o'chirish
            var itemsToRemove = Items.Where(item => item.Quantity <= 0).ToList();
            foreach (var item in itemsToRemove)
            {
                Items.Remove(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulot qo‘shishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button.DataContext as ProductItem;

        if (item != null && ProductGrid.ItemsSource is ObservableCollection<ProductItem> items)
        {
            items.Remove(item);
        }
    }

    private void UpdateTotalSumm()
    {
        try
        {
            double crashSum = ParseDouble(tbCrashSum.Text);
            double qarzSum = ParseDouble(tbqarz.Text);
            double plastikSum = ParseDouble(tbPlastikSum.Text);
            double dollar = ParseDouble(tbDollar.Text);
            double kurs = ParseDouble(tbDolarKurs.Text);
            double chegirma = ParseDouble(tbDiscount.Text);

            tbQoldiq.Text = ((crashSum + plastikSum)/kurs + dollar).ToString("N2");
            double qoldi = ParseDouble(tbQoldiq.Text);

            double gridTotalSum = Items.Sum(item => (double)item.TotalPrice);
            if (gridTotalSum - chegirma < 0 ||
                gridTotalSum - qoldi < 0 ||
                gridTotalSum - qoldi - chegirma < 0)
            {
                string message = "Umumiy summa minus bo'lishi mumkin emas.";
                if (lastUpdatedTextBox != null)
                {
                    lastUpdatedTextBox.Text = null;
                }
                MessageBox.Show(message);
                return;
            }
            tbTotalPrice.Text = (gridTotalSum - qoldi - qarzSum - chegirma).ToString("N2");
        }
        catch
        {
            MessageBox.Show("Bugungi dollar kursini o'rnating.");
        }
    }

    private double ParseDouble(string input)
    {
        double.TryParse(input, out double value);
        return value;
    }

    private void tbCrashSum_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbPlastikSum_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbDollar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (tbDolarKurs.Text == "")
        {
            tbDollar.Text = null;
            MessageBox.Show("Bugungi dollar kursini o'rnating.");
            return;
        }
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbqarz_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);

        if (cbxPayment.SelectedItem == null)
        {
            MessageBox.Show("Iltimos, avval mijozni tanlang!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string text = tbqarz.Text.Trim();
        if (string.IsNullOrWhiteSpace(text) || !decimal.TryParse(text, out decimal amount) || amount <= 0)
        {
            _selectedDebtDate = null;
            return;
        }

        if (_selectedDebtDate.HasValue)
            return;

        DebtPickerWindow datePickerWindow = new DebtPickerWindow();
        if (datePickerWindow.ShowDialog() == true)
        {
            _selectedDebtDate = datePickerWindow.SelectedDate;
        }
    }

    private void tbDiscount_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void TrackTextBoxUpdate(object sender)
    {
        if (sender is TextBox textBox)
        {
            lastUpdatedTextBox = textBox;
        }
    }

    private void ValidateAndCleanInput(object sender)
    {
        if (sender is TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;
            string input = textBox.Text;
            string cleanInput = "";
            bool hasDot = false;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    cleanInput += c;
                }
                else if (c == '.' && !hasDot)
                {
                    cleanInput += c;
                    hasDot = true;
                }
            }

            if (textBox.Text != cleanInput)
            {
                textBox.Text = cleanInput;
                textBox.CaretIndex = Math.Min(caretIndex, cleanInput.Length);
            }
        }
        UpdateTotalSumm();
    }

    private void btnDollarKurs_Click(object sender, RoutedEventArgs e)
    {
        DollarKursWindow dollarKursWindow = new DollarKursWindow(services);
        dollarKursWindow.ShowDialog();
        LoadDollarRate();
    }

    private void tbQuatity_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAndCleanInput(sender);
        if (ProductGrid.SelectedItem is ProductItem selectedItem)
        {
            if (sender is TextBox textBox)
            {
                var dataGridRow = FindParent<DataGridRow>(textBox);
                if (dataGridRow != null)
                {
                    var productItem = dataGridRow.Item as ProductItem;
                    if (productItem != null)
                    {
                        decimal.TryParse(textBox.Text, out decimal newQuantity);
                        if (selectedItem.BalanceAmount < newQuantity)
                        {
                            MessageBox.Show($"{selectedItem.ProductName} " +
                                $"mahsulotning bazadagi qoldiq miqdori: {selectedItem.BalanceAmount} {selectedItem.Unit}");
                            productItem.Quantity = selectedItem.BalanceAmount;
                        }
                        else
                            productItem.Quantity = newQuantity;

                        UpdateTotalSum();
                    }
                }
            }
        }
    }

    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        if (parent == null) return null;

        if (parent is T parentType) return parentType;

        return FindParent<T>(parent);
    }

    private void btnSellDollar_Click(object sender, RoutedEventArgs e)
    {
        DollarSellWindow dollarSellWindow = new DollarSellWindow(services);
        dollarSellWindow.ShowDialog();
    }

    private void btnBuyDollar_Click(object sender, RoutedEventArgs e)
    {
        DollarBuyWindow dollarBuyWindow = new DollarBuyWindow(services);
        dollarBuyWindow.ShowDialog();
    }

    private void cbxPayment_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox == null) return;

        string searchText = comboBox.Text.ToLower();

        if (!string.IsNullOrEmpty(searchText))
        {
            var filteredCustomers = allCustomers
                .Where(c => c.Name.ToLower().Contains(searchText))
                .ToList();

            cbxPayment.ItemsSource = filteredCustomers;
        }
        else
        {
            cbxPayment.ItemsSource = null;
            cbxPayment.ItemsSource = allCustomers;
        }
        cbxPayment.IsDropDownOpen = true;
    }

    private void cbxPayment_LostFocus(object sender, RoutedEventArgs e)
    {
        if (cbxPayment.Template.FindName("PART_EditableTextBox", cbxPayment) is TextBox textBox)
        {
            textBox.SelectionStart = 0;
        }
    }

    private async void btnFinishSale_Click(object sender, RoutedEventArgs e)
    {
        btnFinishSale.IsEnabled = false;

        try
        {
            // ProductGrid bo'sh bo'lsa xabar chiqarish
            if (!Items.Any())
            {
                MessageBox.Show("Avval mahsulot qo'shing!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Quantity 0 yoki undan kichik bo'lgan mahsulotlarni tekshirish
            var invalidItems = Items.Where(item => item.Quantity <= 0).ToList();
            if (invalidItems.Any())
            {
                string invalidProducts = string.Join(", ", invalidItems.Select(item => item.ProductName));
                MessageBox.Show($"Quyidagi mahsulotlarning miqdori 0 yoki undan kichik: {invalidProducts}. Iltimos, miqdorni to'g'rilang!",
                               "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);

                // Birinchi noto'g'ri Quantityga fokus qo'yish
                var firstInvalidItem = invalidItems.First();
                int invalidIndex = Items.IndexOf(firstInvalidItem);
                ProductGrid.SelectedIndex = invalidIndex;
                ProductGrid.ScrollIntoView(firstInvalidItem);

                // Quantity ustunini indeks orqali aniqlash
                int quantityColumnIndex = -1;
                for (int i = 0; i < ProductGrid.Columns.Count; i++)
                {
                    if (ProductGrid.Columns[i].SortMemberPath == "Quantity" || // Property nomi bilan tekshirish
                        ProductGrid.Columns[i].Header?.ToString() == "Quantity") // Header nomi bilan tekshirish
                    {
                        quantityColumnIndex = i;
                        break;
                    }
                }

                if (quantityColumnIndex != -1) // Agar ustun topilsa
                {
                    ProductGrid.CurrentCell = new DataGridCellInfo(firstInvalidItem, ProductGrid.Columns[quantityColumnIndex]);
                    ProductGrid.BeginEdit(); // Tahrirlash rejimiga o'tish
                }
                else
                {
                    // Agar Quantity ustuni topilmasa, umumiy fokus qo'yish
                    ProductGrid.Focus();
                }

                return;
            }

            if (!decimal.TryParse(tbTotalPrice.Text, out decimal totalPrice) || totalPrice != 0)
            {
                MessageBox.Show("To‘lovni to‘liq amalga oshiring!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedCustomerId == null || SelectedCustomerId <= 0)
            {
                var dialog = new CustomerSelectionDialog();
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.SelectCustomer)
                    {
                        cbxPayment.Focus();
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            var selectedItem = cbxPayment.SelectedItem as ComboboxCustomerItem;
            if (selectedItem == null && (SelectedCustomerId != null && SelectedCustomerId > 0))
            {
                MessageBox.Show("Tanlangan element topilmadi!", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saleDto = new SaleCreationDto
            {
                CustomerId = selectedItem?.CustomerId > 0 ? selectedItem.CustomerId : null,
                UserId = selectedItem?.UserId > 0 ? selectedItem.UserId : null,
                TotalAmount = ProductGrid.Items.Cast<ProductItem>().Sum(p => p.TotalPrice),
                Payments = new List<PaymentCreationDto>(),
                Debts = new List<DebtCreationDto>()
            };

            var createdSale = await _saleService.AddAsync(saleDto);
            if (createdSale == null)
            {
                MessageBox.Show("Sotuvni yaratishda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var product in ProductGrid.Items.Cast<ProductItem>())
            {
                var saleItemDto = new SaleItemCreationDto
                {
                    SaleId = createdSale.Id,
                    ProductId = product.ProductId,
                    UnitOfMeasure = product.Unit,
                    Quantity = product.Quantity,
                    UnitPrice = product.Price
                };
                var addedSaleItem = await _saleItemService.AddAsync(saleItemDto);
                if (addedSaleItem == null)
                {
                    MessageBox.Show($"SaleItem qo‘shishda xatolik: {product.ProductName}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var priceToUpdate = await _priceService.RetrieveByIdAsync(product.PriceId);
                if (priceToUpdate != null)
                {
                    var updatedQuantity = priceToUpdate.Quantity - product.Quantity;
                    if (updatedQuantity < 0)
                    {
                        MessageBox.Show($"{product.ProductName} uchun yetarli qoldiq yo‘q!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var priceUpdateDto = new PriceUpdateDto
                    {
                        Id = priceToUpdate.Id,
                        ProductId = priceToUpdate.ProductId,
                        CostPrice = priceToUpdate.CostPrice,
                        SellingPrice = priceToUpdate.SellingPrice,
                        UnitOfMeasure = priceToUpdate.UnitOfMeasure,
                        Quantity = updatedQuantity
                    };
                    var isPriceUpdated = await _priceService.ModifyAsync(priceUpdateDto);
                    if (!isPriceUpdated)
                    {
                        MessageBox.Show($"{product.ProductName} narxini yangilashda xatolik!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show($"{product.ProductName} uchun narx topilmadi (PriceId: {product.PriceId})!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var cashRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            if (cashRegisters == null || !cashRegisters.Any())
            {
                MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var currentRegister = cashRegisters.First();

            decimal uzsBalance = currentRegister.UzsBalance;
            decimal uzpBalance = currentRegister.UzpBalance;
            decimal usdBalance = currentRegister.UsdBalance;
            string customerName = selectedItem?.Name ?? "Noma'lum mijoz";

            if (decimal.TryParse(tbCrashSum.Text, out decimal cashAmount) && cashAmount > 0)
            {
                var cashDto = new PaymentCreationDto { SaleId = createdSale.Id, Amount = cashAmount, Type = PaymentType.Cash };
                await _paymentService.AddAsync(cashDto);
                uzsBalance += cashAmount;

                var cashTransferDto = new CashTransferCreationDto
                {
                    CashRegisterId = currentRegister.Id,
                    From = "Mijoz",
                    To = "Kassa",
                    Currency = "So'm",
                    Amount = cashAmount,
                    Note = $"Sotuv #{createdSale.Id} - Naqd to‘lov (Mijoz: {customerName})",
                    TransferDate = DateTimeOffset.UtcNow,
                    TransferType = CashTransferType.Income
                };
                await _cashTransferService.AddAsync(cashTransferDto);
            }

            if (decimal.TryParse(tbPlastikSum.Text, out decimal cardAmount) && cardAmount > 0)
            {
                var cardDto = new PaymentCreationDto { SaleId = createdSale.Id, Amount = cardAmount, Type = PaymentType.Card };
                await _paymentService.AddAsync(cardDto);
                uzpBalance += cardAmount;

                var cardTransferDto = new CashTransferCreationDto
                {
                    CashRegisterId = currentRegister.Id,
                    From = "Mijoz",
                    To = "Kassa",
                    Currency = "Plastik",
                    Amount = cardAmount,
                    Note = $"Sotuv #{createdSale.Id} - Plastik to‘lov (Mijoz: {customerName})",
                    TransferDate = DateTimeOffset.UtcNow,
                    TransferType = CashTransferType.Income
                };
                await _cashTransferService.AddAsync(cardTransferDto);
            }

            if (decimal.TryParse(tbDollar.Text, out decimal dollarAmount) && dollarAmount > 0)
            {
                var dollarDto = new PaymentCreationDto { SaleId = createdSale.Id, Amount = dollarAmount, Type = PaymentType.Dollar };
                await _paymentService.AddAsync(dollarDto);
                usdBalance += dollarAmount;

                var dollarTransferDto = new CashTransferCreationDto
                {
                    CashRegisterId = currentRegister.Id,
                    From = "Mijoz",
                    To = "Kassa",
                    Currency = "Dollar",
                    Amount = dollarAmount,
                    Note = $"Sotuv #{createdSale.Id} - Dollar to‘lov (Mijoz: {customerName})",
                    TransferDate = DateTimeOffset.UtcNow,
                    TransferType = CashTransferType.Income
                };
                await _cashTransferService.AddAsync(dollarTransferDto);
            }

            if (decimal.TryParse(tbqarz.Text, out decimal debtAmount) && debtAmount > 0)
            {
                var debtPaymentDto = new PaymentCreationDto { SaleId = createdSale.Id, Amount = debtAmount, Type = PaymentType.Debt };
                await _paymentService.AddAsync(debtPaymentDto);

                if (!_selectedDebtDate.HasValue)
                {
                    MessageBox.Show("Qarz uchun to‘lash sanasi tanlanmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var debtDto = new DebtCreationDto
                {
                    SaleId = createdSale.Id,
                    RemainingAmount = debtAmount,
                    DueDate = _selectedDebtDate.Value.ToUniversalTime()
                };
                await _debtService.AddAsync(debtDto);

                DebtEvents.RaiseDebtUpdated();
            }

            if (decimal.TryParse(tbDiscount.Text, out decimal discountAmount) && discountAmount > 0)
            {
                var discountDto = new DiscountCreationDto { SaleId = createdSale.Id, Amount = discountAmount };
                await _discountService.AddAsync(discountDto);
            }

            var updatedRegister = new CashRegisterUpdateDto
            {
                Id = currentRegister.Id,
                UzsBalance = uzsBalance,
                UzpBalance = uzpBalance,
                UsdBalance = usdBalance
            };
            var isRegisterUpdated = await _cashRegisterService.ModifyAsync(updatedRegister);
            if (isRegisterUpdated == null)
            {
                MessageBox.Show("Kassa balansini yangilashda xatolik!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saleItemsToPrint = ProductGrid.Items.Cast<ProductItem>().Select(product => new SaleItemPrintModel
            {
                SerialNumber = product.SerialNumber,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                Unit = product.Unit,
                TotalPrice = product.TotalPrice
            }).ToList();

            var printDialog = new PrintOrExportDialog();
            if (printDialog.ShowDialog() == true)
            {
                try
                {
                    if (printDialog.PrintSelected)
                    {
                        PrintSaleItems(saleItemsToPrint, createdSale, selectedItem);
                    }
                    else
                    {
                        ExportToExcel(saleItemsToPrint, createdSale, selectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Print yoki Excelga o‘tkazishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            MessageBox.Show("Sotuv muvaffaqiyatli amalga oshirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

            SaleCompleted?.Invoke(this, EventArgs.Empty);

            CashEvents.RaiseCashUpdated();
        }
        finally
        {
            btnFinishSale.IsEnabled = true;
            ResetToDefaultValues();
        }
    }
    private void ResetToDefaultValues()
    {
        tbCrashSum.Text = "";
        tbPlastikSum.Text = "";
        tbDollar.Text = "";
        tbqarz.Text = "";
        tbDiscount.Text = "";
        tbQuantity.Text = "";

        tbQoldiq.Text = "0.00";
        tbTotalPrice.Text = "0.00";

        cbxProduct.SelectedItem = null;
        cbxPayment.SelectedItem = null;
        SelectedCustomerId = null;

        Items.Clear();

        _selectedDebtDate = null;
        dpDueDate.SelectedDate = null;
        dpDueDate.Visibility = Visibility.Collapsed;

        cbxProduct.Focus();
    }

    private void ExportToExcel(List<SaleItemPrintModel> saleItems, SaleResultDto sale, ComboboxCustomerItem customer)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sotuv Cheki");

            worksheet.Cell(1, 1).Value = "Sotuv Cheki";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 20;
            worksheet.Range("A1:E1").Merge();

            worksheet.Cell(2, 1).Value = $"Mijoz: {customer?.Name ?? ""}";
            worksheet.Cell(2, 1).Style.Font.FontSize = 14;

            worksheet.Cell(4, 1).Value = "Mahsulot";
            worksheet.Cell(4, 2).Value = "Narx";
            worksheet.Cell(4, 3).Value = "Miqdor";
            worksheet.Cell(4, 4).Value = "Birlik";
            worksheet.Cell(4, 5).Value = "Jami";
            worksheet.Range("A4:E4").Style.Font.Bold = true;

            int row = 5;
            foreach (var item in saleItems)
            {
                worksheet.Cell(row, 1).Value = item.ProductName;
                worksheet.Cell(row, 2).Value = item.Price;
                worksheet.Cell(row, 3).Value = item.Quantity;
                worksheet.Cell(row, 4).Value = item.Unit;
                worksheet.Cell(row, 5).Value = item.TotalPrice;
                row++;
            }

            worksheet.Cell(row, 4).Value = "Jami";
            worksheet.Cell(row, 5).Value = sale.TotalAmount;
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00"; // Dollar belgisi
            worksheet.Range(row, 4, row, 5).Style.Font.Bold = true;
            row++;

            if (decimal.TryParse(tbCrashSum.Text, out decimal cashAmount) && cashAmount > 0)
            {
                worksheet.Cell(row, 4).Value = "Naqd to‘lov";
                worksheet.Cell(row, 5).Value = cashAmount;
                row++;
            }

            if (decimal.TryParse(tbPlastikSum.Text, out decimal cardAmount) && cardAmount > 0)
            {
                worksheet.Cell(row, 4).Value = "Plastik to‘lov";
                worksheet.Cell(row, 5).Value = cardAmount;
                row++;
            }

            if (decimal.TryParse(tbDollar.Text, out decimal dollarAmount) && dollarAmount > 0)
            {
                worksheet.Cell(row, 4).Value = "Dollar to‘lov";
                worksheet.Cell(row, 5).Value = dollarAmount;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00";
                row++;
            }

            // Chegirma (dollar formatida)
            if (decimal.TryParse(tbDiscount.Text, out decimal discountAmount) && discountAmount > 0)
            {
                worksheet.Cell(row, 4).Value = "Chegirma";
                worksheet.Cell(row, 5).Value = discountAmount;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00"; // Dollar belgisi
                row++;
            }

            // To‘lov summasi (dollar formatida)
            if (decimal.TryParse(tbQoldiq.Text, out decimal paymentSum) && paymentSum > 0)
            {
                worksheet.Cell(row, 4).Value = "To‘lov summasi";
                worksheet.Cell(row, 5).Value = paymentSum;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00"; // Dollar belgisi
                worksheet.Range(row, 4, row, 5).Style.Font.Bold = true;
                row++;
            }

            // Qarz (dollar formatida)
            if (decimal.TryParse(tbqarz.Text, out decimal debtAmount) && debtAmount > 0)
            {
                worksheet.Cell(row, 4).Value = "Qarz";
                worksheet.Cell(row, 5).Value = debtAmount;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00"; // Dollar belgisi
                worksheet.Range(row, 4, row, 5).Style.Font.Bold = true;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = $"Sotuv_Cheki_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excel fayliga muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    private void PrintSaleItems(List<SaleItemPrintModel> saleItems, SaleResultDto sale, ComboboxCustomerItem customer)
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

            Paragraph customerInfo = new Paragraph(new Run($"Mijoz: {customer?.Name ?? ""}"))
            {
                FontSize = 14,
                Margin = new Thickness(0, 10, 0, 0)
            };
            doc.Blocks.Add(customerInfo);

            Table table = new Table();
            table.CellSpacing = 5;
            table.BorderThickness = new Thickness(1);
            table.BorderBrush = Brushes.Black;

            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });

            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Mahsulot")) { FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Narx")) { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Right });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Miqdor")) { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Right });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Birlik")) { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Center });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Jami")) { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Right });
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
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 50, 0),
                TextAlignment = TextAlignment.Right
            };
            doc.Blocks.Add(totalAmount);

            if (decimal.TryParse(tbCrashSum.Text, out decimal cashAmount) && cashAmount > 0)
            {
                Paragraph cashPayment = new Paragraph(new Run($"Naqd to‘lov: {cashAmount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(cashPayment);
            }

            if (decimal.TryParse(tbPlastikSum.Text, out decimal cardAmount) && cardAmount > 0)
            {
                Paragraph cardPayment = new Paragraph(new Run($"Plastik to‘lov: {cardAmount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(cardPayment);
            }

            if (decimal.TryParse(tbDollar.Text, out decimal dollarAmount) && dollarAmount > 0)
            {
                Paragraph dollarPayment = new Paragraph(new Run($"Dollar to‘lov: ${dollarAmount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(dollarPayment);
            }

            if (decimal.TryParse(tbDiscount.Text, out decimal discountAmount) && discountAmount > 0)
            {
                Paragraph discount = new Paragraph(new Run($"Chegirma: ${discountAmount:N2}"))
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(discount);
            }

            if (decimal.TryParse(tbQoldiq.Text, out decimal paymentSum) && paymentSum > 0)
            {
                Paragraph paymentTotal = new Paragraph(new Run($"To‘lov summasi: ${paymentSum:N2}"))
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(paymentTotal);
            }

            if (decimal.TryParse(tbqarz.Text, out decimal debtAmount) && debtAmount > 0)
            {
                Paragraph debt = new Paragraph(new Run($"Qarz: ${debtAmount:N2}"))
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 50, 0),
                    TextAlignment = TextAlignment.Right
                };
                doc.Blocks.Add(debt);
            }

            printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Sotuv Cheki");
        }
    }

    private void cbxPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxPayment.SelectedItem is ComboboxCustomerItem selectedItem)
        {
            if (selectedItem.CustomerId > 0)
            {
                SelectedCustomerId = selectedItem.CustomerId;
            }
            else if (selectedItem.UserId > 0)
            {
                SelectedCustomerId = selectedItem.UserId;
            }
            else
            {
                SelectedCustomerId = null;
            }
        }
        else
        {
            SelectedCustomerId = null;
        }
    }
}