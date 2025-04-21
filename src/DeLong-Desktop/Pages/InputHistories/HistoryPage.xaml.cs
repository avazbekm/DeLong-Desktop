using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Cashs;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Enums;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Suppliers;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

namespace DeLong_Desktop.Pages.InputHistories;

/// <summary>
/// Interaction logic for HistoryPage.xaml
/// </summary>
public partial class HistoryPage : Page
{
    private readonly IServiceProvider _services;
    private readonly ICreditorDebtService _creditorDebtService;
    private readonly ICreditorDebtPaymentService _creditorDebtPaymentService;
    private readonly ISupplierService _supplierService;
    private readonly ITransactionItemService _transactionItemService;
    private readonly IUserService _userService;
    private readonly ICashRegisterService _cashRegisterService;
    private readonly ICashTransferService _cashTransferService;
    private List<HistoryItem> _historyItems = new();

    public HistoryPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _creditorDebtService = services.GetRequiredService<ICreditorDebtService>();
        _creditorDebtPaymentService = services.GetRequiredService<ICreditorDebtPaymentService>();
        _supplierService = services.GetRequiredService<ISupplierService>();
        _transactionItemService = services.GetRequiredService<ITransactionItemService>();
        _userService = services.GetRequiredService<IUserService>();
        _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
        _cashTransferService = services.GetRequiredService<ICashTransferService>();
        AppState.CurrentHistoryPage = this;

        LoadSuppliersAsync();
        LoadHistoryAsync();
    }

    public async void LoadHistoryAsync()
    {
        try
        {
            var debts = await _creditorDebtService.RetrieveAllAsync();
            _historyItems = debts.Select(d => new HistoryItem
            {
                DebtId = d.Id,
                TransactionId = d.TransactionId, // Taxmin: CreditorDebtDto da TransactionId bor
                SupplierId = d.SupplierId,
                SupplierName = d.SupplierName.ToUpper(),
                Date = d.Date,
                TotalAmount = d.RemainingAmount + d.CreditorDebtPayments.Sum(p => p.Amount),
                PaidAmount = d.CreditorDebtPayments.Sum(p => p.Amount),
                RemainingAmount = d.RemainingAmount,
                Status = d.IsSettled ? "To‘langan" : d.CreditorDebtPayments.Any() ? "Qisman to‘langan" : "To‘lanmagan"
            })
            .OrderByDescending(d => d.Date)
            .ToList();

            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tarixni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LoadSuppliersAsync()
    {
        try
        {
            var suppliers = await _supplierService.RetrieveAllAsync() ?? new List<SupplierResultDto>();
            var supplierItems = new List<SupplierItem> { new SupplierItem { Id = 0, Name = "Hammasi" } };
            supplierItems.AddRange(suppliers.Select(s => new SupplierItem { Id = s.Id, Name = s.Name }));

            cbxSupplier.ItemsSource = supplierItems;
            cbxSupplier.SelectedIndex = 0;

            cbxStatus.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Value = "Hammasi" },
                new StatusItem { Value = "To‘langan" },
                new StatusItem { Value = "Qisman to‘langan" },
                new StatusItem { Value = "To‘lanmagan" }
            };
            cbxStatus.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Taminotchilarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFilters()
    {
        var filteredItems = _historyItems.AsEnumerable();

        var searchText = txtSearch.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(searchText))
        {
            filteredItems = filteredItems.Where(i => i.TransactionId.ToString().Contains(searchText) || i.SupplierName.ToLower().Contains(searchText));
        }

        if (dpStartDate.SelectedDate.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.Date.Date >= dpStartDate.SelectedDate.Value);
        }
        if (dpEndDate.SelectedDate.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.Date.Date <= dpEndDate.SelectedDate.Value);
        }

        if (cbxSupplier.SelectedItem is SupplierItem supplier && supplier.Id > 0)
        {
            filteredItems = filteredItems.Where(i => i.SupplierId == supplier.Id);
        }

        if (cbxStatus.SelectedItem is StatusItem status && status.Value != "Hammasi")
        {
            filteredItems = filteredItems.Where(i => i.Status == status.Value);
        }

        historyDataGrid.ItemsSource = filteredItems.ToList();
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void Filter_Changed(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilters();
    }

    private void DetailsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is HistoryItem item)
        {
            try
            {
                var detailsWindow = new DetailsWindow(item, _creditorDebtService, _creditorDebtPaymentService, _transactionItemService, _userService);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tafsilotlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void historyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (historyDataGrid.SelectedItem is HistoryItem)
        {
            txtPaymentAmount.IsEnabled = true;
            txtComment.IsEnabled = true;
            btnPay.IsEnabled = true;
        }
        else
        {
            txtPaymentAmount.IsEnabled = false;
            txtComment.IsEnabled = false;
            btnPay.IsEnabled = false;
            txtPaymentAmount.Text = string.Empty;
            txtComment.Text = string.Empty;
        }
    }

    private void txtPaymentAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !decimal.TryParse(e.Text, out _) && e.Text != ".";
    }

    private async void PayButton_Click(object sender, RoutedEventArgs e)
    {
        if (historyDataGrid.SelectedItem is not HistoryItem selectedItem)
        {
            MessageBox.Show("Iltimos, to‘lov qilish uchun qarzni tanlang!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(txtPaymentAmount.Text, out decimal paymentAmount) || paymentAmount <= 0)
        {
            MessageBox.Show("Iltimos, to‘g‘ri to‘lov summasini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            btnPay.IsEnabled = false; // Takroriy bosishni oldini olish

            // 1. Ochiq kassani olish
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            var cashRegister = openRegisters.FirstOrDefault();
            if (cashRegister == null)
            {
                MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Dollar qoldig‘ini tekshirish
            if (cashRegister.UsdBalance < paymentAmount)
            {
                MessageBox.Show($"Kassada yetarli dollar mavjud emas! Joriy qoldiq: {cashRegister.UsdBalance:N2} USD", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal remainingPayment = paymentAmount;
            string comment = txtComment.Text.Trim();
            var paymentDate = DateTimeOffset.UtcNow;

            // 3. Tanlangan qarzga to‘lov
            if (selectedItem.RemainingAmount > 0)
            {
                decimal amountToPay = Math.Min(remainingPayment, selectedItem.RemainingAmount);
                if (amountToPay > 0)
                {
                    var paymentDto = new CreditorDebtPaymentCreationDto
                    {
                        CreditorDebtId = selectedItem.DebtId,
                        Amount = amountToPay,
                        PaymentDate = paymentDate,
                        Description = string.IsNullOrEmpty(comment) ? null : comment
                    };
                    await _creditorDebtPaymentService.AddAsync(paymentDto);

                    // CashTransfer yozish
                    var transferDto = new CashTransferCreationDto
                    {
                        CashRegisterId = cashRegister.Id,
                        From = "Kassa",
                        To = selectedItem.SupplierName,
                        Currency = "Dollar",
                        Amount = amountToPay,
                        Note = $"{selectedItem.SupplierName} taminotchiga qarz to‘landi. TransactionId={selectedItem.TransactionId}",
                        TransferType = CashTransferType.Expense,
                        TransferDate = paymentDate
                    };
                    await _cashTransferService.AddAsync(transferDto);

                    remainingPayment -= amountToPay;
                }
            }

            // 4. Boshqa qarzlarga to‘lov
            if (remainingPayment > 0)
            {
                var debts = await _creditorDebtService.RetrieveAllAsync();
                var unpaidDebts = debts
                    .Where(d => d.Id != selectedItem.DebtId && d.RemainingAmount > 0)
                    .OrderBy(d => d.Date)
                    .ToList();

                foreach (var debt in unpaidDebts)
                {
                    if (remainingPayment <= 0) break;

                    decimal amountToPay = Math.Min(remainingPayment, debt.RemainingAmount);
                    if (amountToPay > 0)
                    {
                        var paymentDto = new CreditorDebtPaymentCreationDto
                        {
                            CreditorDebtId = debt.Id,
                            Amount = amountToPay,
                            PaymentDate = paymentDate,
                            Description = string.IsNullOrEmpty(comment) ? null : comment
                        };
                        await _creditorDebtPaymentService.AddAsync(paymentDto);

                        // CashTransfer yozish
                        var transferDto = new CashTransferCreationDto
                        {
                            CashRegisterId = cashRegister.Id,
                            From = "Kassa",
                            To = debt.SupplierName,
                            Currency = "Dollar",
                            Amount = amountToPay,
                            Note = $"{debt.SupplierName} taminotchiga qarz to‘landi. TransactionId={debt.TransactionId}",
                            TransferType = CashTransferType.Expense,
                            TransferDate = paymentDate
                        };
                        await _cashTransferService.AddAsync(transferDto);

                        remainingPayment -= amountToPay;
                    }
                }
            }

            // 5. CashRegister ni yangilash
            var updatedRegister = new CashRegisterUpdateDto
            {
                Id = cashRegister.Id,
                UzsBalance = cashRegister.UzsBalance,
                UzpBalance = cashRegister.UzpBalance,
                UsdBalance = cashRegister.UsdBalance - paymentAmount, // To‘lov summasi ayiriladi
                ClosedAt = cashRegister.ClosedAt
            };
            await _cashRegisterService.ModifyAsync(updatedRegister);

            // 6. CashPage ni yangilash uchun event qo‘zg‘atish
            CashEvents.RaiseCashUpdated();

            // 7. Natija xabari
            if (remainingPayment > 0)
            {
                MessageBox.Show($"To‘lov muvaffaqiyatli amalga oshirildi! Qaytarib berish summasi: {remainingPayment:N2} USD", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("To‘lov muvaffaqiyatli amalga oshirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // 8. UI ni tozalash va yangilash
            txtPaymentAmount.Text = string.Empty;
            txtComment.Text = string.Empty;
            LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"To‘lov amalga oshirishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            btnPay.IsEnabled = true; // Tugma qayta faollashadi
        }
    }
}