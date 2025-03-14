using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.DTOs;
using System.Collections.ObjectModel;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.Pages.AdditianalOperations;
using DeLong_Desktop.ApiService.DTOs.DebtPayments;
using DeLong_Desktop.ApiService.DTOs.Transactions;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.Pages.AdditionalOperations;

public partial class AdditionalOperationsPage : Page
{
    private readonly IDebtService _debtService;
    private readonly IUserService _userService;
    private readonly ISaleService _saleService;
    private readonly IPriceService _priceService;
    private readonly IProductService _productService;
    private readonly ISaleItemService _saleItemService;
    private readonly ICustomerService _customerService;
    private readonly IWarehouseService _warehouseService; // Omborlar uchun servis
    private readonly IKursDollarService _kursDollarService;
    private readonly IDebtPaymentService _debtPaymentService;
    private readonly ITransactionService _transactionService;
    private readonly IReturnProductService _returnProductService;
    private readonly ITransactionItemService _transactionItemService;

    private List<DebtItem> allDebts;
    private List<DebtItem> selectedCustomerDebts;

    // DataGrid uchun ObservableCollection
    public ObservableCollection<TransferItem> TransferItems { get; set; } = new();
    private List<ComboboxItem> allProducts = new(); // Barcha mahsulotlar ro‘yxati saqlanadi

    public AdditionalOperationsPage(IServiceProvider services)
    {
        InitializeComponent();
        _debtService = services.GetRequiredService<IDebtService>();
        _userService = services.GetRequiredService<IUserService>();
        _saleService = services.GetRequiredService<ISaleService>();
        _priceService = services.GetRequiredService<IPriceService>();
        _productService = services.GetRequiredService<IProductService>();
        _customerService = services.GetRequiredService<ICustomerService>();
        _saleItemService = services.GetRequiredService<ISaleItemService>();
        _warehouseService = services.GetRequiredService<IWarehouseService>();
        _kursDollarService = services.GetRequiredService<IKursDollarService>();
        _transactionService = services.GetRequiredService<ITransactionService>();
        _debtPaymentService = services.GetRequiredService<IDebtPaymentService>();
        _returnProductService = services.GetRequiredService<IReturnProductService>();
        _transactionItemService = services.GetRequiredService<ITransactionItemService>();

        LoadDebts();
        LoadSalePriceProducts();
        // DataGrid ga bog‘lash
        transferDataGrid.ItemsSource = TransferItems;

        // Mahsulotlar ro‘yxatini yuklash
        LoadProductData();
        LoadWarehouses(); // Omborlarni yuklash
        cbTransactionType.ItemsSource = Enum.GetNames(typeof(TransactionType));
    }

    private async void LoadWarehouses()
    {
        try
        {
            // Omborlar ro‘yxatini olish
            var warehouses = await _warehouseService.RetrieveAllAsync();
            if (warehouses == null || !warehouses.Any())
            {
                MessageBox.Show("Omborlar topilmadi! API yoki ma’lumot bazasini tekshiring.");
                return;
            }

            // ComboBox’ga omborlarni yuklash
            cbToWarehouse.ItemsSource = warehouses;
            cbToWarehouse.DisplayMemberPath = "Name"; // Ombor nomini ko‘rsatish
            cbToWarehouse.SelectedValuePath = "Id";   // Ombor ID’sini saqlash
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Omborlarni yuklashda xatolik: {ex.Message}");
        }
    }

    // Mahsulotlar ro‘yxatini ComboBox ga yuklash
    private async void LoadProductData()
    {
        try
        {
            var products = await _productService.RetrieveAllAsync();
            if (products == null || !products.Any())
            {
                MessageBox.Show("Mahsulotlar topilmadi! API yoki ma’lumot bazasini tekshiring.");
                return;
            }

            allProducts = products.Select(product => new ComboboxItem
            {
                Id = product.Id,
                ProductName = char.ToUpper(product.Name[0]) + product.Name.Substring(1)
            }).ToList();

            cbProductList.ItemsSource = allProducts;
            cbProductList.DisplayMemberPath = "ProductName";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlarni yuklashda xatolik: {ex.Message}");
        }
    }
    // "Qo‘shish" tugmasi uchun metod
    private async void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cbProductList.SelectedItem is not ComboboxItem selectedProduct ||
                !decimal.TryParse(tbQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Mahsulotni tanlang yoki to‘g‘ri miqdor kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    var newItem = new TransferItem
                    {
                        Id = price.Id,
                        ProductId = price.ProductId,
                        ProductName = selectedProduct.ProductName,
                        Quantity = quantity,
                        Unit = price.UnitOfMeasure,
                        UnitPrice = price.SellingPrice
                    };

                    TransferItems.Add(newItem);
                    break;
                }
                else 
                {
                    var newItem = new TransferItem
                    {
                        Id = price.Id,
                        ProductId = price.ProductId,
                        ProductName = selectedProduct.ProductName,
                        Quantity = price.Quantity,
                        Unit = price.UnitOfMeasure,
                        UnitPrice = price.SellingPrice
                    };

                    TransferItems.Add(newItem);
                    quantity-= price.Quantity;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulot qo‘shishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // cbProductList uchun qidiruv logikasi
    private void cbProductList_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox == null) return;

        string searchText = comboBox.Text.Trim().ToLower();

        if (!string.IsNullOrEmpty(searchText))
        {
            var filteredProducts = allProducts
                .Where(p => p.ProductName.ToLower().Contains(searchText))
                .ToList();

            cbProductList.ItemsSource = filteredProducts;
        }
        else
        {
            cbProductList.ItemsSource = allProducts; // Agar qidiruv bo‘sh bo‘lsa, barcha mahsulotlarni qaytarish
        }

        cbProductList.IsDropDownOpen = true; // Har doim ochiq qoladi
    }

    private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is TransferItem item)
            {
                TransferItems.Remove(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"O‘chirishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private async void LoadDebts()
    {
        try
        {
            allDebts = new List<DebtItem>();

            var groupedDebts = await _debtService.RetrieveAllGroupedByCustomerAsync();
            if (groupedDebts != null && groupedDebts.Any())
            {
                foreach (var customer in groupedDebts)
                {
                    foreach (var debt in customer.Value)
                    {
                        var debtItem = new DebtItem
                        {
                            Id = debt.Id,
                            DueDate = debt.DueDate.ToString("dd.MM.yyyy"),
                            DueDateValue = debt.DueDate,
                            RemainingAmount = debt.RemainingAmount,
                            CustomerName = customer.Key.ToUpper()
                        };
                        allDebts.Add(debtItem);
                    }
                }
            }
            else
            {
                allDebts = new List<DebtItem>();
            }

            debtDataGrid.ItemsSource = allDebts;
            // Tanlangan mijozning qarzlarini yangilash
            DebtDataGrid_SelectionChanged(null, null);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Qarzlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void SearchDebt_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = tbSearchDebt.Text.ToLower();

        var filteredDebts = allDebts.Where(debt =>
        {
            bool matchesCustomerName = !string.IsNullOrEmpty(debt.CustomerName) && debt.CustomerName.ToLower().Contains(searchText);
            bool matchesAmount = debt.RemainingAmount.ToString().Contains(searchText);
            return matchesCustomerName || matchesAmount;
        }).ToList();

        debtDataGrid.ItemsSource = filteredDebts;
    }
    private void DebtDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (debtDataGrid.SelectedItem is DebtItem selectedDebt)
        {
            // Tanlangan mijozning barcha qarzlarini yig‘amiz
            selectedCustomerDebts = allDebts
                .Where(debt => debt.CustomerName == selectedDebt.CustomerName)
                .OrderBy(debt => debt.DueDateValue) // DateTimeOffset bo‘yicha tartiblash
                .ToList();

            // Jami qarzni hisoblaymiz
            decimal totalDebt = selectedCustomerDebts.Sum(debt => debt.RemainingAmount);
            tbTotalDebt.Text = $"{totalDebt:N2} so‘m";
        }
        else
        {
            selectedCustomerDebts = new List<DebtItem>();
            tbTotalDebt.Text = "0.00 so‘m";
        }
    }
    private async void PayDebtButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is DebtItem selectedDebt)
        {
            // To‘lov summalarini olish
            if (!decimal.TryParse(tbCashPayment.Text, out decimal cashAmount) || cashAmount < 0)
                cashAmount = 0;
            if (!decimal.TryParse(tbCardPayment.Text, out decimal cardAmount) || cardAmount < 0)
                cardAmount = 0;
            if (!decimal.TryParse(tbDollarPayment.Text, out decimal dollarAmount) || dollarAmount < 0)
                dollarAmount = 0;

            // Umumiy to‘lov summasini hisoblash
            if (cashAmount == 0 && cardAmount == 0 && dollarAmount == 0)
            {
                MessageBox.Show($"Qator ID: {selectedDebt.Id} uchun to‘g‘ri to‘lov summasini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Dollar kursini olish
                var dollar = await _kursDollarService.RetrieveByIdAsync();
                if (dollar == null && dollarAmount > 0)
                {
                    MessageBox.Show("Dollar kursini kiriting", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                decimal dollarKurs = dollar?.AdmissionDollar ?? 0;
                decimal dollarInSom = dollarAmount * dollarKurs;

                // Umumiy to‘lov summasi (so‘mda)
                decimal totalPayment = cashAmount + cardAmount + dollarInSom;

                // Agar to‘lov summasi qarz summasidan ko‘p bo‘lsa, xato
                if (totalPayment > selectedDebt.RemainingAmount)
                {
                    MessageBox.Show($"To‘lov summasi ({totalPayment:N2} so‘m) qarz summasidan ({selectedDebt.RemainingAmount:N2} so‘m) ko‘p bo‘lishi mumkin emas!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Har bir to‘lov usulini alohida saqlash
                if (cashAmount > 0)
                {
                    var cashPaymentDto = new DebtPaymentCreationDto
                    {
                        DebtId = selectedDebt.Id,
                        Amount = cashAmount,
                        PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                        PaymentMethod = "Cash"
                    };
                    await _debtPaymentService.AddAsync(cashPaymentDto);
                }

                if (cardAmount > 0)
                {
                    var cardPaymentDto = new DebtPaymentCreationDto
                    {
                        DebtId = selectedDebt.Id,
                        Amount = cardAmount,
                        PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                        PaymentMethod = "Card"
                    };
                    await _debtPaymentService.AddAsync(cardPaymentDto);
                }

                if (dollarAmount > 0)
                {
                    var dollarPaymentDto = new DebtPaymentCreationDto
                    {
                        DebtId = selectedDebt.Id,
                        Amount = dollarInSom, // So‘mga aylantirilgan summa
                        PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                        PaymentMethod = "Dollar"
                    };
                    await _debtPaymentService.AddAsync(dollarPaymentDto);
                }

                MessageBox.Show("To‘lov muvaffaqiyatli amalga oshirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDebts();
                tbCashPayment.Text = null;
                tbCardPayment.Text = null;
                tbDollarPayment.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Qator ID: {selectedDebt.Id} uchun to‘lov amalga oshirishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private async void PayAllDebtsButton_Click(object sender, RoutedEventArgs e)
    {
        if (selectedCustomerDebts == null || !selectedCustomerDebts.Any())
        {
            MessageBox.Show("Iltimos, avval mijozni tanlang!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (!decimal.TryParse(tbCashPayment.Text, out decimal cashAmount) || cashAmount < 0)
            cashAmount = 0;
        if (!decimal.TryParse(tbCardPayment.Text, out decimal cardAmount) || cardAmount < 0)
            cardAmount = 0;
        if (!decimal.TryParse(tbDollarPayment.Text, out decimal dollarAmount) || dollarAmount < 0)
            dollarAmount = 0;

        if (cashAmount == 0 && cardAmount == 0 && dollarAmount == 0)
        {
            MessageBox.Show("Iltimos, kamida bitta to‘lov summasini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            // Dollar kursini olish
            var dollar = await _kursDollarService.RetrieveByIdAsync();
            if (dollar == null)
            {
                MessageBox.Show("Dollar kursini kiriting", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            decimal dollarKurs = dollar.AdmissionDollar;
            decimal dollarInSom = dollarAmount * dollarKurs;

            // Umumiy to‘lov summasi (so‘mda)
            decimal totalPayment = cashAmount + cardAmount + dollarInSom;

            // Jami qarzni tekshirish
            decimal totalDebt = selectedCustomerDebts.Sum(debt => debt.RemainingAmount);
            if (totalPayment > totalDebt)
            {
                MessageBox.Show($"To‘lov summasi ({totalPayment:N2} so‘m) jami qarz summasidan ({totalDebt:N2} so‘m) ko‘p bo‘lishi mumkin emas!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Muddati bo‘yicha tartiblangan qarzlar
            decimal remainingPayment = totalPayment;

            foreach (var debt in selectedCustomerDebts)
            {
                if (remainingPayment <= 0)
                    break;

                decimal amountToPay = Math.Min(debt.RemainingAmount, remainingPayment);
                if (amountToPay > 0)
                {
                    decimal remainingAmountToPay = amountToPay;

                    // Naqd to‘lov
                    if (remainingAmountToPay > 0 && cashAmount > 0)
                    {
                        decimal cashToPay = Math.Min(cashAmount, remainingAmountToPay);
                        var cashPaymentDto = new DebtPaymentCreationDto
                        {
                            DebtId = debt.Id,
                            Amount = cashToPay,
                            PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                            PaymentMethod = "Cash"
                        };
                        await _debtPaymentService.AddAsync(cashPaymentDto);
                        cashAmount -= cashToPay;
                        remainingAmountToPay -= cashToPay;
                    }

                    // Plastik to‘lov
                    if (remainingAmountToPay > 0 && cardAmount > 0)
                    {
                        decimal cardToPay = Math.Min(cardAmount, remainingAmountToPay);
                        var cardPaymentDto = new DebtPaymentCreationDto
                        {
                            DebtId = debt.Id,
                            Amount = cardToPay,
                            PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                            PaymentMethod = "Card"
                        };
                        await _debtPaymentService.AddAsync(cardPaymentDto);
                        cardAmount -= cardToPay;
                        remainingAmountToPay -= cardToPay;
                    }

                    // Dollar to‘lov
                    if (remainingAmountToPay > 0 && dollarAmount > 0)
                    {
                        decimal remainingDollarInSom = dollarAmount * dollarKurs;
                        decimal dollarToPayInSom = Math.Min(remainingDollarInSom, remainingAmountToPay);
                        if (dollarToPayInSom > 0)
                        {
                            var dollarPaymentDto = new DebtPaymentCreationDto
                            {
                                DebtId = debt.Id,
                                Amount = dollarToPayInSom,
                                PaymentDate = DateTimeOffset.Now.ToUniversalTime(),
                                PaymentMethod = "Dollar"
                            };
                            await _debtPaymentService.AddAsync(dollarPaymentDto);
                            decimal usedDollars = dollarToPayInSom / dollarKurs;
                            dollarAmount -= usedDollars;
                            remainingAmountToPay -= dollarToPayInSom;
                        }
                    }

                    remainingPayment -= amountToPay;
                }
            }
            MessageBox.Show("To‘lov muvaffaqiyatli amalga oshirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDebts();
            tbCashPayment.Text = null;
            tbCardPayment.Text = null;
            tbDollarPayment.Text = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"To‘lov amalga oshirishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void AcceptReturnedProductButton_Click(object sender, RoutedEventArgs e) { }
    private void AddReturnedProductButton_Click(object sender, RoutedEventArgs e) { }
    private void TransferProductButton_Click(object sender, RoutedEventArgs e) { }

    private void tbCashPayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void tbCardPayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void tbDollarPayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private async void LoadSalePriceProducts()
    {
        //try
        //{
        //    // SalePrice dan mahsulotlar ro‘yxatini olish (taxminiy xizmat)
        //    var salePrices = await _salePriceService.RetrieveAllAsync(); // Bu metod sizning loyihangizda bo‘lishi kerak
        //    if (salePrices != null && salePrices.Any())
        //    {
        //        cbSalePriceProducts.ItemsSource = salePrices.Select(sp => new
        //        {
        //            ProductId = sp.ProductId,
        //            ProductName = sp.ProductName // Mahsulot nomi
        //        }).ToList();
        //        cbSalePriceProducts.DisplayMemberPath = "ProductName";
        //        cbSalePriceProducts.SelectedValuePath = "ProductId";
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Mahsulotlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        //}
    }

    private async void ConfirmReturnButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!long.TryParse(tbSaleId.Text, out long saleId) || string.IsNullOrEmpty(tbSaleId.Text))
            {
                MessageBox.Show("Iltimos, Chek ID kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var returnedFrom = tbReturnedFrom.Text;
            var selectedProduct = cbSalePriceProducts.SelectedItem as dynamic;
            if (selectedProduct == null)
            {
                MessageBox.Show("Iltimos, mahsulot tanlang!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(tbReturnQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Iltimos, to‘g‘ri miqdor kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var unitOfMeasure = tbUnitOfMeasure.Text;
            if (string.IsNullOrEmpty(unitOfMeasure))
            {
                MessageBox.Show("Iltimos, o‘lchov birligini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(tbReturnAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Iltimos, to‘g‘ri summa kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var comment = tbComment.Text.ToLower();
            var returnDto = new ReturnProductCreationDto
            {
                SaleId = saleId,
                ProductId = selectedProduct.ProductId,
                ProductName = selectedProduct.ProductName.ToLower(),
                Quatity = quantity,
                UnitOfMeasure = unitOfMeasure.ToLower(),
                ReturnSumma = amount,
                Reason = comment
            };

            var existCustomerId = await _saleService.RetrieveByIdAsync(saleId);
            if (existCustomerId != null)
            {
                if (existCustomerId.CustomerId.HasValue && existCustomerId.CustomerId > 0)
                {
                    returnDto.CustomerId = existCustomerId.CustomerId.Value;
                }
                else if (existCustomerId.UserId.HasValue && existCustomerId.UserId > 0)
                {
                    returnDto.UserId = existCustomerId.UserId.Value;
                }
            }

            var result = await _returnProductService.AddAsync(returnDto);

            if (result != null)
            {
                MessageBox.Show("Qaytgan mahsulot muvaffaqiyatli tasdiqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                tbSaleId.Text = "";
                tbReturnedFrom.Text = "";
                cbSalePriceProducts.SelectedIndex = -1;
                tbReturnQuantity.Text = "";
                tbUnitOfMeasure.Text = "";
                tbReturnAmount.Text = "";
                tbComment.Text = "";
            }
            else
            {
                MessageBox.Show("Qaytgan mahsulotni tasdiqlashda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void tbSaleId_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private async void tbSaleId_LostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tbSaleId.Text) || !long.TryParse(tbSaleId.Text, out long saleId))
            {
                cbSalePriceProducts.ItemsSource = null;
                tbReturnedFrom.Text = string.Empty;
                return;
            }

            // SaleItemService dan saleId orqali sale itemlarni olamiz
            var saleItems = await _saleItemService.RetrieveAllBySaleIdAsync(saleId);

            if (saleItems != null && saleItems.Any())
            {
                // Mahsulot nomini ProductService dan olish
                var productNames = await Task.WhenAll(saleItems.Select(async item =>
                {
                    var product = await _productService.RetrieveByIdAsync(item.ProductId);
                    return new
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name.ToUpper() ?? "Noma'lum mahsulot"
                    };
                }));

                cbSalePriceProducts.ItemsSource = productNames;
                cbSalePriceProducts.DisplayMemberPath = "ProductName";
                cbSalePriceProducts.SelectedValuePath = "ProductId";
                cbSalePriceProducts.SelectedIndex = -1;
            }
            else
            {
                cbSalePriceProducts.ItemsSource = null;
                MessageBox.Show("Bu ID bilan hech qanday mahsulot topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // SaleService dan mijoz nomini olamiz
            var saleDetails = await _saleService.RetrieveByIdAsync(saleId);
            if (saleDetails != null && saleDetails.CustomerId.HasValue && saleDetails.CustomerId > 0)
            {
                var customer = await _customerService.RetrieveByIdAsync(saleDetails.CustomerId.Value);
                tbReturnedFrom.Text = customer?.Name.ToUpper() ?? string.Empty;
            }
            else if (saleDetails != null && saleDetails.UserId.HasValue && saleDetails.UserId > 0)
            {
                var user = await _userService.RetrieveByIdAsync(saleDetails.UserId.Value);
                tbReturnedFrom.Text = $"{user?.FirstName.ToUpper() ?? ""} {user?.LastName.ToUpper() ?? ""}".Trim();
            }
            else
            {
                tbReturnedFrom.Text = "Noma'lum mijoz";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            cbSalePriceProducts.ItemsSource = null;
            tbReturnedFrom.Text = string.Empty;
        }
    }

    // Mahsulot nomini olish uchun taxminiy funksiya (real loyihada ProductService ishlatilishi mumkin)
    private void tbReturnQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void tbReturnAmount_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private async void SaveTransferButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Validatsiya: DataGridda mahsulot borligini tekshirish
            if (TransferItems.Count == 0)
            {
                MessageBox.Show("Hech qanday mahsulot qo‘shilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validatsiya: Ombor va tranzaksiya turi tanlanganligini tekshirish
            if (cbToWarehouse.SelectedItem == null || cbTransactionType.SelectedItem == null)
            {
                MessageBox.Show("Ombor yoki tranzaksiya turini tanlang!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tranzaksiya DTO yaratish
            var transactionDto = new TransactionCreationDto
            {
                WarehouseIdTo = (long)cbToWarehouse.SelectedValue,
                TransactionType = (TransactionType)Enum.Parse(typeof(TransactionType), cbTransactionType.SelectedItem.ToString()),
                Comment = tbCommentProvodka.Text
            };

            // Tranzaksiyani yaratish
            var createdTransaction = await _transactionService.AddAsync(transactionDto);
            if (createdTransaction == null)
            {
                MessageBox.Show("Tranzaksiya yaratishda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // DataGriddagi har bir qator uchun tranzaksiya itemlarini qo‘shish
            foreach (var item in TransferItems)
            {
                var transactionItemDto = new TransactionItemCreationDto
                {
                    TransactionId  = createdTransaction.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceProduct = item.UnitPrice,
                    UnitOfMeasure = item.Unit
                };

                await _transactionItemService.AddAsync(transactionItemDto);

                var price = await _priceService.RetrieveByIdAsync(item.Id);
                PriceUpdateDto priceUpdateDto = new PriceUpdateDto
                {
                    Id = item.Id,
                    CostPrice = price.CostPrice,
                    ProductId = price.ProductId,
                    SellingPrice = price.SellingPrice,
                    UnitOfMeasure = price.UnitOfMeasure,
                    Quantity = price.Quantity - item.Quantity
                };
                await _priceService.ModifyAsync(priceUpdateDto);
            }

            // Muvaffaqiyatli saqlangandan so‘ng ro‘yxatni tozalash
            MessageBox.Show("Tranzaksiya muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            TransferItems.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tranzaksiya saqlashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}