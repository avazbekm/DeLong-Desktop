using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Enums;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;

namespace DeLong_Desktop.Pages.Cashs;

public partial class CashPage : Page
{
    private readonly ICashRegisterService _cashRegisterService;
    private readonly ICashTransferService _cashTransferService;
    private readonly ICashWarehouseService _cashWarehouseService;
    private readonly IUserService _userService;
    private long? _currentCashRegisterId = null;

    public CashPage(IServiceProvider services)
    {
        InitializeComponent();
        _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
        _cashTransferService = services.GetRequiredService<ICashTransferService>();
        _cashWarehouseService = services.GetRequiredService<ICashWarehouseService>();
        _userService = services.GetRequiredService<IUserService>();

        FromComboBox.SelectionChanged -= FromComboBox_SelectionChanged;
        LoadWarehouseData();
        LoadCashData();
        Loaded += CashPage_Loaded;
        FromComboBox.SelectionChanged += FromComboBox_SelectionChanged;

        CashEvents.CashUpdated += async (s, e) => await RefreshCashData();
    }

    public async Task RefreshCashData()
    {
        LoadCashData();
        LoadWarehouseData();
    }

    private async void CashPage_Loaded(object sender, RoutedEventArgs e)
    {
        await UpdateButtonState();
    }

    private async void LoadWarehouseData()
    {
        var latestWarehouse = await _cashWarehouseService.RetrieveByIdAsync();
        if (latestWarehouse == null)
        {
            CashWarehouseCreationDto cashWarehouseCreationDto = new CashWarehouseCreationDto();
            latestWarehouse = await _cashWarehouseService.AddAsync(cashWarehouseCreationDto);
        }
        CashWarehouseGrid.ItemsSource = null;
        CashWarehouseGrid.ItemsSource = new List<CashWarehouseResultDto> { latestWarehouse };
    }

    private async void LoadCashData()
    {
        var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
        if (openRegisters?.Any() == true)
        {
            var reg = openRegisters.First();
        }
        CashRegisterGrid.ItemsSource = null;
        CashRegisterGrid.ItemsSource = openRegisters;

        if (openRegisters == null || !openRegisters.Any())
        {
            MessageBox.Show("Hozirda ochiq kassalar yo‘q!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void TransferButton_Click(object sender, RoutedEventArgs e)
    {
        string from = (FromComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        string to = (ToComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        if ((from == "Zaxiradan" && to == "Zaxiraga") || (from == "Kassadan" && to == "Kassaga") || from == to)
        {
            MessageBox.Show("Qayerdan va qayerga o'tkazish bir xil bo'lmasligi kerak!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (from == "Kassadan" && to != "Zaxiraga" && to != "Boshqa")
        {
            MessageBox.Show("Kassadan faqat Zaxiraga yoki Boshqa ga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (from == "Zaxiradan" && to != "Kassaga")
        {
            MessageBox.Show("Zaxiradan faqat Kassaga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (from == "Boshqa" && to != "Kassaga")
        {
            MessageBox.Show("Boshqa dan faqat Kassaga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
        {
            MessageBox.Show("Miqdor 0 dan katta bo'lishi kerak va to'g'ri kiritilishi lozim!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string currency = (CurrencyComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

        var note = NoteTextBox.Text;
        if (string.IsNullOrEmpty(note))
        {
            MessageBox.Show("Izohni to'ldiring", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
        var warehouse = await _cashWarehouseService.RetrieveByIdAsync();

        if (openRegisters == null || !openRegisters.Any())
        {
            MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var firstRegister = openRegisters.First();

        decimal availableBalance = 0;
        if (from == "Zaxiradan")
        {
            if (warehouse == null)
            {
                MessageBox.Show("Zaxira ma'lumotlari topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            switch (currency)
            {
                case "So'm":
                    availableBalance = warehouse.UzsBalance;
                    break;
                case "Plastik":
                    availableBalance = warehouse.UzpBalance;
                    break;
                case "Dollar":
                    availableBalance = warehouse.UsdBalance;
                    break;
            }
        }
        else if (from == "Kassadan")
        {
            switch (currency)
            {
                case "So'm":
                    availableBalance = firstRegister.UzsBalance;
                    break;
                case "Plastik":
                    availableBalance = firstRegister.UzpBalance;
                    break;
                case "Dollar":
                    availableBalance = firstRegister.UsdBalance;
                    break;
            }
        }

        if (from != "Boshqa" && amount > availableBalance)
        {
            MessageBox.Show($"Yetarli mablag' yo'q! {currency} bo'yicha joriy balans: {availableBalance}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            await PerformTransfer(from, to, currency, amount, note, firstRegister.Id);
            MessageBox.Show($"O'tkazma muvaffaqiyatli: {amount} {currency} {from} dan {to} ga.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadCashData();
            LoadWarehouseData();
            AmountTextBox.Text = null;
            NoteTextBox.Text = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task PerformTransfer(string from, string to, string currency, decimal amount, string note, long cashRegisterId)
    {
        var transferDto = new CashTransferCreationDto
        {
            CashRegisterId = cashRegisterId,
            From = from == "Zaxiradan" ? "Zaxira" : (from == "Kassadan" ? "Kassa" : "Boshqa"),
            To = to == "Zaxiraga" ? "Zaxira" : (to == "Kassaga" ? "Kassa" : "Boshqa"),
            Currency = currency,
            Amount = amount,
            Note = note,
            TransferDate = DateTimeOffset.UtcNow,
            TransferType = (from == "Zaxiradan" && to == "Kassaga") || (from == "Boshqa" && to == "Kassaga")
                ? CashTransferType.Income
                : CashTransferType.Expense
        };

        await _cashTransferService.AddAsync(transferDto);

        var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
        var warehouse = await _cashWarehouseService.RetrieveByIdAsync();
        var register = openRegisters.First(r => r.Id == cashRegisterId);

        if (from == "Zaxiradan" && to == "Kassaga")
        {
            await UpdateBalances(warehouse, register, currency, -amount, amount);
        }
        else if (from == "Kassadan" && to == "Zaxiraga")
        {
            await UpdateBalances(warehouse, register, currency, amount, -amount);
        }
        else if (from == "Boshqa" && to == "Kassaga")
        {
            await UpdateBalances(warehouse, register, currency, 0, amount);
        }
        else if (from == "Kassadan" && to == "Boshqa")
        {
            await UpdateBalances(warehouse, register, currency, 0, -amount);
        }
    }

    private async Task UpdateBalances(CashWarehouseResultDto warehouse, CashRegisterResultDto register, string currency, decimal warehouseChange, decimal registerChange)
    {
        var updatedWarehouse = new CashWarehouseUpdateDto
        {
            Id = warehouse.Id,
            UzsBalance = warehouse.UzsBalance,
            UzpBalance = warehouse.UzpBalance,
            UsdBalance = warehouse.UsdBalance
        };

        var updatedRegister = new CashRegisterUpdateDto
        {
            Id = register.Id,
            UzsBalance = register.UzsBalance,
            UzpBalance = register.UzpBalance,
            UsdBalance = register.UsdBalance,
        };

        switch (currency)
        {
            case "So'm":
                updatedWarehouse.UzsBalance += warehouseChange;
                updatedRegister.UzsBalance += registerChange;
                break;
            case "Plastik":
                updatedWarehouse.UzpBalance += warehouseChange;
                updatedRegister.UzpBalance += registerChange;
                break;
            case "Dollar":
                updatedWarehouse.UsdBalance += warehouseChange;
                updatedRegister.UsdBalance += registerChange;
                break;
        }

        await _cashWarehouseService.ModifyAsync(updatedWarehouse);
        await _cashRegisterService.ModifyAsync(updatedRegister);
    }

    private void NoteTextBox_GotFocus(object sender, RoutedEventArgs e) { }

    private void AmountTextBox_GotFocus(object sender, RoutedEventArgs e) { }

    private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e) { }

    private void NoteTextBox_LostFocus(object sender, RoutedEventArgs e) { }

    private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void FromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FromComboBox == null || ToComboBox == null)
            return;

        if (FromComboBox.SelectedItem is ComboBoxItem item && item.Content != null)
        {
            string selectedFrom = item.Content.ToString();
            ToComboBox.Items.Clear();

            if (selectedFrom == "Kassadan")
            {
                ToComboBox.Items.Add(new ComboBoxItem { Content = "Zaxiraga" });
                ToComboBox.Items.Add(new ComboBoxItem { Content = "Boshqa" });
                ToComboBox.SelectedIndex = 0;
                ToComboBox.IsEnabled = true;
            }
            else if (selectedFrom == "Zaxiradan" || selectedFrom == "Boshqa")
            {
                ToComboBox.Items.Add(new ComboBoxItem { Content = "Kassaga" });
                ToComboBox.SelectedIndex = 0;
                ToComboBox.IsEnabled = false;
            }
        }
    }

    private async void ShowTurnoverButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string selectedCurrency = (CurrencyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() switch
            {
                "So'm (UZS)" => "So'm",
                "Plastik (UZP)" => "Plastik",
                "Dollar (USD)" => "Dollar",
                _ => "So'm"
            };

            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            if (openRegisters == null || !openRegisters.Any())
            {
                MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var currentRegister = openRegisters.First();
            var openedAt = currentRegister.OpenedAt;

            var transfers = await _cashTransferService.RetrieveAllAsync();
            if (transfers == null || !transfers.Any())
            {
                MessageBox.Show("Kassa aylanmasi ma’lumotlari topilmadi!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
                TurnoverListView.Visibility = Visibility.Collapsed;
                viewListHeader.Visibility = Visibility.Hidden;
                totalSummary.Visibility = Visibility.Hidden;
                return;
            }

            var filteredTransfers = new List<object>();
            int sequenceNumber = 1;

            foreach (var transfer in transfers
                .Where(t => t.CashRegisterId == currentRegister.Id &&
                            t.TransferDate >= openedAt &&
                            t.Currency == selectedCurrency)
                .OrderBy(t => t.TransferDate))
            {
                string executorName = "Noma'lum";
                if (transfer.CreatedBy > 0)
                {
                    var user = await _userService.RetrieveByIdAsync(transfer.CreatedBy);
                    if (user != null)
                    {
                        executorName = $"{user.FirstName} {user.LastName}".Trim();
                    }
                }

                filteredTransfers.Add(new
                {
                    SequenceNumber = sequenceNumber++,
                    From = transfer.From,
                    To = transfer.To,
                    Note = transfer.Note,
                    Income = transfer.TransferType == CashTransferType.Income ? transfer.Amount : 0,
                    Expense = transfer.TransferType == CashTransferType.Expense ? transfer.Amount : 0,
                    TransferDate = transfer.TransferDate,
                    ExecutorName = executorName
                });
            }

            TurnoverListView.ItemsSource = filteredTransfers;
            TurnoverListView.Visibility = Visibility.Visible;
            viewListHeader.Visibility = Visibility.Visible;
            totalSummary.Visibility = Visibility.Visible;

            decimal totalIncome = filteredTransfers.Sum(t => (decimal)t.GetType().GetProperty("Income").GetValue(t));
            decimal totalExpense = filteredTransfers.Sum(t => (decimal)t.GetType().GetProperty("Expense").GetValue(t));

            tbTotalIncome.Text = totalIncome.ToString("N0");
            tbTotalExpense.Text = totalExpense.ToString("N0");

            if (!filteredTransfers.Any())
            {
                MessageBox.Show($"Tanlangan valyuta ({selectedCurrency}) bo‘yicha aylanma topilmadi!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ShowHistoryTurnoverButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Sanalarni tekshirish
            if (!HistoryStartDatePicker.SelectedDate.HasValue || !HistoryEndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Iltimos, boshlang‘ich va tugash sanalarni tanlang!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var startDate = HistoryStartDatePicker.SelectedDate.Value;
            var endDate = HistoryEndDatePicker.SelectedDate.Value.AddDays(1).AddTicks(-1); // Tugash sanasi kun oxirigacha

            if (startDate > endDate)
            {
                MessageBox.Show("Boshlang‘ich sana tugash sanadan katta bo‘lmasligi kerak!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedCurrency = (HistoryCurrencyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() switch
            {
                "So'm (UZS)" => "So'm",
                "Plastik (UZP)" => "Plastik",
                "Dollar (USD)" => "Dollar",
                _ => "So'm"
            };

            var transfers = await _cashTransferService.RetrieveAllAsync();
            if (transfers == null || !transfers.Any())
            {
                MessageBox.Show("Kassa aylanmasi ma’lumotlari topilmadi!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
                HistoryTurnoverListView.Visibility = Visibility.Collapsed;
                historyViewListHeader.Visibility = Visibility.Hidden;
                historyTotalSummary.Visibility = Visibility.Hidden;
                return;
            }

            var filteredTransfers = new List<object>();
            int sequenceNumber = 1;

            foreach (var transfer in transfers
                .Where(t => t.TransferDate >= startDate && t.TransferDate <= endDate && t.Currency == selectedCurrency)
                .OrderBy(t => t.TransferDate))
            {
                string executorName = "Noma'lum";
                if (transfer.CreatedBy > 0)
                {
                    var user = await _userService.RetrieveByIdAsync(transfer.CreatedBy);
                    if (user != null)
                    {
                        executorName = $"{user.FirstName} {user.LastName}".Trim();
                    }
                }

                filteredTransfers.Add(new
                {
                    SequenceNumber = sequenceNumber++,
                    From = transfer.From,
                    To = transfer.To,
                    Note = transfer.Note,
                    Income = transfer.TransferType == CashTransferType.Income ? transfer.Amount : 0,
                    Expense = transfer.TransferType == CashTransferType.Expense ? transfer.Amount : 0,
                    TransferDate = transfer.TransferDate,
                    ExecutorName = executorName
                });
            }

            HistoryTurnoverListView.ItemsSource = filteredTransfers;
            HistoryTurnoverListView.Visibility = Visibility.Visible;
            historyViewListHeader.Visibility = Visibility.Visible;
            historyTotalSummary.Visibility = Visibility.Visible;

            decimal totalIncome = filteredTransfers.Sum(t => (decimal)t.GetType().GetProperty("Income").GetValue(t));
            decimal totalExpense = filteredTransfers.Sum(t => (decimal)t.GetType().GetProperty("Expense").GetValue(t));

            tbHistoryTotalIncome.Text = totalIncome.ToString("N0");
            tbHistoryTotalExpense.Text = totalExpense.ToString("N0");

            if (!filteredTransfers.Any())
            {
                MessageBox.Show($"Tanlangan valyuta ({selectedCurrency}) va sanalar bo‘yicha aylanma topilmadi!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OpenDayButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            if (openRegisters != null && openRegisters.Any())
            {
                var currentRegister = openRegisters.First();
                _currentCashRegisterId = currentRegister.Id;

                var confirm = MessageBox.Show("E'tiborli bo'ling kunni yopishni xohlaysizmi?", "Tasdiqlash",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    var warehouse = await _cashWarehouseService.RetrieveByIdAsync();
                    if (warehouse == null)
                    {
                        var newWarehouseDto = new CashWarehouseCreationDto();
                        warehouse = await _cashWarehouseService.AddAsync(newWarehouseDto);
                        if (warehouse == null)
                        {
                            MessageBox.Show("Zaxira omborini yaratishda xatolik!", "Xatolik",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if (currentRegister.UzsBalance != 0 || currentRegister.UzpBalance != 0 || currentRegister.UsdBalance != 0)
                    {
                        var resetBalance = MessageBox.Show("Balanslar 0 emas! Qoldiqlarni zaxiraga o‘tkazib, balansni 0 qilishni xohlaysizmi?",
                            "Balans tekshiruvi", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (resetBalance == MessageBoxResult.Yes)
                        {
                            if (currentRegister.UzsBalance > 0)
                            {
                                var uzsTransfer = new CashTransferCreationDto
                                {
                                    CashRegisterId = currentRegister.Id,
                                    From = "Kassa",
                                    To = "Zaxira",
                                    Currency = "UZS",
                                    Amount = currentRegister.UzsBalance,
                                    Note = "Kun yopilganda UZS qoldiq zaxiraga o‘tkazildi",
                                    TransferType = CashTransferType.Expense,
                                    TransferDate = DateTimeOffset.UtcNow
                                };
                                await _cashTransferService.AddAsync(uzsTransfer);
                            }
                            if (currentRegister.UzpBalance > 0)
                            {
                                var uzpTransfer = new CashTransferCreationDto
                                {
                                    CashRegisterId = currentRegister.Id,
                                    From = "Kassa",
                                    To = "Zaxira",
                                    Currency = "UZP",
                                    Amount = currentRegister.UzpBalance,
                                    Note = "Kun yopilganda UZP qoldiq zaxiraga o‘tkazildi",
                                    TransferType = CashTransferType.Expense,
                                    TransferDate = DateTimeOffset.UtcNow
                                };
                                await _cashTransferService.AddAsync(uzpTransfer);
                            }
                            if (currentRegister.UsdBalance > 0)
                            {
                                var usdTransfer = new CashTransferCreationDto
                                {
                                    CashRegisterId = currentRegister.Id,
                                    From = "Kassa",
                                    To = "Zaxira",
                                    Currency = "USD",
                                    Amount = currentRegister.UsdBalance,
                                    Note = "Kun yopilganda USD qoldiq zaxiraga o‘tkazildi",
                                    TransferType = CashTransferType.Expense,
                                    TransferDate = DateTimeOffset.UtcNow
                                };
                                await _cashTransferService.AddAsync(usdTransfer);
                            }

                            var updateWarehouseDto = new CashWarehouseUpdateDto
                            {
                                Id = warehouse.Id,
                                UzsBalance = warehouse.UzsBalance + currentRegister.UzsBalance,
                                UzpBalance = warehouse.UzpBalance + currentRegister.UzpBalance,
                                UsdBalance = warehouse.UsdBalance + currentRegister.UsdBalance
                            };
                            var updatedWarehouse = await _cashWarehouseService.ModifyAsync(updateWarehouseDto);
                            if (updatedWarehouse == null)
                            {
                                MessageBox.Show("Zaxira balansini yangilashda xatolik!", "Xatolik",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            var updateDto = new CashRegisterUpdateDto
                            {
                                Id = currentRegister.Id,
                                UzsBalance = 0,
                                UzpBalance = 0,
                                UsdBalance = 0,
                                ClosedAt = DateTimeOffset.UtcNow
                            };
                            var updatedRegister = await _cashRegisterService.ModifyAsync(updateDto);
                            if (updatedRegister != null)
                            {
                                MessageBox.Show("Qoldiqlar zaxiraga o‘tkazildi va kun muvaffaqiyatli yopildi!",
                                    "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                                await UpdateButtonState();
                                CashRegisterGrid.ItemsSource = await _cashRegisterService.RetrieveOpenRegistersAsync();
                                LoadWarehouseData();
                            }
                            else
                            {
                                MessageBox.Show("Kunni yopishda xatolik yuz berdi!", "Xatolik",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        var updateDto = new CashRegisterUpdateDto
                        {
                            Id = currentRegister.Id,
                            UzsBalance = 0,
                            UzpBalance = 0,
                            UsdBalance = 0,
                            ClosedAt = DateTimeOffset.UtcNow
                        };
                        var updatedRegister = await _cashRegisterService.ModifyAsync(updateDto);
                        if (updatedRegister != null)
                        {
                            MessageBox.Show("Kun muvaffaqiyatli yopildi!", "Muvaffaqiyat",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            await UpdateButtonState();
                            CashRegisterGrid.ItemsSource = await _cashRegisterService.RetrieveOpenRegistersAsync();
                        }
                        else
                        {
                            MessageBox.Show("Kunni yopishda xatolik yuz berdi!", "Xatolik",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                var newCashRegister = new CashRegisterCreationDto
                {
                    UzsBalance = 0,
                    UzpBalance = 0,
                    UsdBalance = 0
                };

                var createdRegister = await _cashRegisterService.AddAsync(newCashRegister);
                if (createdRegister != null)
                {
                    _currentCashRegisterId = createdRegister.Id;
                    MessageBox.Show("Kun muvaffaqiyatli ochildi!", "Muvaffaqiyat",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await UpdateButtonState();
                    CashRegisterGrid.ItemsSource = await _cashRegisterService.RetrieveOpenRegistersAsync();
                }
                else
                {
                    MessageBox.Show("Kassa ochishda xatolik yuz berdi!", "Xatolik",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task UpdateButtonState()
    {
        var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
        if (openRegisters != null && openRegisters.Any())
        {
            OpenDayButton.Content = "Kunni yopish";
            OpenDayButton.Foreground = new SolidColorBrush(Colors.Yellow);
            _currentCashRegisterId = openRegisters.First().Id;
        }
        else
        {
            OpenDayButton.Content = "Kunni ochish";
            OpenDayButton.Foreground = new SolidColorBrush(Colors.Snow);
            _currentCashRegisterId = null;
        }
    }
}