using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.Pages.AdditianalOperations;
using DeLong_Desktop.ApiService.DTOs.DebtPayments;

namespace DeLong_Desktop.Pages.AdditionalOperations;

public partial class AdditionalOperationsPage : Page
{
    private readonly IDebtService _debtService;
    private readonly IUserService _userService;
    private readonly ISaleService _saleService;
    private readonly ICustomerService _customerService;
    private readonly IKursDollarService _kursDollarService;
    private readonly IDebtPaymentService _debtPaymentService;
    private List<DebtItem> allDebts;
    private List<DebtItem> selectedCustomerDebts;

    public AdditionalOperationsPage(IServiceProvider services)
    {
        InitializeComponent();
        _debtService = services.GetRequiredService<IDebtService>();
        _userService = services.GetRequiredService<IUserService>();
        _saleService = services.GetRequiredService<ISaleService>();
        _customerService = services.GetRequiredService<ICustomerService>();
        _kursDollarService = services.GetRequiredService<IKursDollarService>();
        _debtPaymentService = services.GetRequiredService<IDebtPaymentService>();
        LoadDebts();
    }

    private async void LoadDebts()
    {
        try
        {
            allDebts = new List<DebtItem>(); // Dastlab ro‘yxatni yaratamiz

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
}