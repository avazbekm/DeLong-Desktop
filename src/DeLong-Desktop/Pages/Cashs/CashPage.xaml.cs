using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Enums;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;

namespace DeLong_Desktop.Pages.Cashs
{
    public partial class CashPage : Page
    {
        private readonly ICashRegisterService _cashRegisterService;
        private readonly ICashTransferService _cashTransferService;
        private readonly ICashWarehouseService _cashWarehouseService;

        public CashPage(IServiceProvider services)
        {
            InitializeComponent();
            _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
            _cashTransferService = services.GetRequiredService<ICashTransferService>();
            _cashWarehouseService = services.GetRequiredService<ICashWarehouseService>();
            // SelectionChanged hodisasini vaqtincha o‘chirish
            FromComboBox.SelectionChanged -= FromComboBox_SelectionChanged;
            LoadData();
            LoadWarehouseData();
            FromComboBox.SelectionChanged += FromComboBox_SelectionChanged; // Yuklashdan keyin qayta ulash
        }

        private async void LoadWarehouseData()
        {
            var latestWarehouse = await _cashWarehouseService.RetrieveByIdAsync();
            CashWarehouseGrid.ItemsSource = new List<CashWarehouseResultDto> { latestWarehouse };
        }

        private async void LoadData()
        {
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            CashRegisterGrid.ItemsSource = openRegisters;

            if (openRegisters == null || !openRegisters.Any())
            {
                MessageBox.Show("Hozirda ochiq kassalar yo‘q!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. FromComboBox va ToComboBox qiymatlarini olish
            string from = (FromComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string to = (ToComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 2. Bir xil bo‘lmasligini va ruxsat etilmagan kombinatsiyalarni tekshirish
            if ((from == "Zaxiradan" && to == "Zaxiraga") || (from == "Kassadan" && to == "Kassaga") || from == to)
            {
                MessageBox.Show("Qayerdan va qayerga o'tkazish bir xil bo'lmasligi kerak!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. "Kassadan" faqat "Zaxiraga" yoki "Boshqa" ga bo‘lishini tekshirish
            if (from == "Kassadan" && to != "Zaxiraga" && to != "Boshqa")
            {
                MessageBox.Show("Kassadan faqat Zaxiraga yoki Boshqa ga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. "Zaxiradan" faqat "Kassaga" ga bo‘lishini tekshirish
            if (from == "Zaxiradan" && to != "Kassaga")
            {
                MessageBox.Show("Zaxiradan faqat Kassaga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 5. "Boshqa" faqat "Kassaga" ga bo‘lishini tekshirish
            if (from == "Boshqa" && to != "Kassaga")
            {
                MessageBox.Show("Boshqa dan faqat Kassaga o'tkazish mumkin!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 6. AmountTextBox qiymati 0 bo'lmasligini tekshirish
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Miqdor 0 dan katta bo'lishi kerak va to'g'ri kiritilishi lozim!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 7. Valyutani aniqlash
            string currency = (CurrencyComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 8. Izohni olish
            var note = NoteTextBox.Text;
            if (string.IsNullOrEmpty(note))
            {
                MessageBox.Show("Izohni to'ldiring", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 9. Balanslarni xizmatlardan olish
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            var warehouse = await _cashWarehouseService.RetrieveByIdAsync();

            if (openRegisters == null || !openRegisters.Any())
            {
                MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var firstRegister = openRegisters.First();

            // 10. Balansni tekshirish (faqat "Kassadan" yoki "Zaxiradan" uchun)
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

            // 11. Miqdor yetarli ekanligini tekshirish (faqat "Zaxiradan" yoki "Kassadan" uchun)
            if (from != "Boshqa" && amount > availableBalance)
            {
                MessageBox.Show($"Yetarli mablag' yo'q! {currency} bo'yicha joriy balans: {availableBalance}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 12. O'tkazma amalga oshirish
            try
            {
                await PerformTransfer(from, to, currency, amount, note, firstRegister.Id);
                MessageBox.Show($"O'tkazma muvaffaqiyatli: {amount} {currency} {from} dan {to} ga.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
                LoadWarehouseData();
                AmountTextBox.Text = null;
                NoteTextBox.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // O'tkazmani amalga oshirish va balanslarni yangilash uchun funksiya
        private async Task PerformTransfer(string from, string to, string currency, decimal amount, string note, long cashRegisterId)
        {
            // 1. Transfer DTO yaratish
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

            // 2. O'tkazmani serverga yuborish
            await _cashTransferService.AddAsync(transferDto);

            // 3. Balanslarni yangilash
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            var warehouse = await _cashWarehouseService.RetrieveByIdAsync();
            var register = openRegisters.First(r => r.Id == cashRegisterId);

            if (from == "Zaxiradan" && to == "Kassaga")
            {
                // Zaxiradan Kassaga
                await UpdateBalances(warehouse, register, currency, -amount, amount);
            }
            else if (from == "Kassadan" && to == "Zaxiraga")
            {
                // Kassadan Zaxiraga
                await UpdateBalances(warehouse, register, currency, amount, -amount);
            }
            else if (from == "Boshqa" && to == "Kassaga")
            {
                // Boshqadan Kassaga (faqat kassaga kirim)
                await UpdateBalances(warehouse, register, currency, 0, amount);
            }
            else if (from == "Kassadan" && to == "Boshqa")
            {
                // Kassadan Boshqa (faqat kassadan chiqim)
                await UpdateBalances(warehouse, register, currency, 0, -amount);
            }
        }
        // Balanslarni yangilash uchun yordamchi funksiya
        private async Task UpdateBalances(CashWarehouseResultDto warehouse, CashRegisterResultDto register, string currency, decimal warehouseChange, decimal registerChange)
        {
            // Zaxira va kassa uchun yangi DTO'lar tayyorlash
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

            // Valyutaga qarab balanslarni o'zgartirish
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

            // Yangilangan ma'lumotlarni serverga yuborish
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
            // Agar FromComboBox yoki ToComboBox null bo‘lsa, hech narsa qilmaymiz
            if (FromComboBox == null || ToComboBox == null)
                return;

            // SelectedItem null emasligini tekshirish
            if (FromComboBox.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                string selectedFrom = item.Content.ToString();
                ToComboBox.Items.Clear(); // ToComboBox ni tozalash

                if (selectedFrom == "Kassadan")
                {
                    ToComboBox.Items.Add(new ComboBoxItem { Content = "Zaxiraga" });
                    ToComboBox.Items.Add(new ComboBoxItem { Content = "Boshqa" });
                    ToComboBox.SelectedIndex = 0; // "Zaxiraga" default
                    ToComboBox.IsEnabled = true;  // Tanlash mumkin
                }
                else if (selectedFrom == "Zaxiradan" || selectedFrom == "Boshqa")
                {
                    ToComboBox.Items.Add(new ComboBoxItem { Content = "Kassaga" });
                    ToComboBox.SelectedIndex = 0; // "Kassaga" default
                    ToComboBox.IsEnabled = false; // Faqat "Kassaga"
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

                var filteredTransfers = transfers
                    .Where(t => t.CashRegisterId == currentRegister.Id &&
                                t.TransferDate >= openedAt &&
                                t.Currency == selectedCurrency)
                    .OrderBy(t => t.TransferDate)
                    .Select((t, index) => new
                    {
                        SequenceNumber = index + 1,
                        From = t.From,
                        To = t.To,
                        Note = t.Note,
                        Income = t.TransferType == CashTransferType.Income ? t.Amount : 0,
                        Expense = t.TransferType == CashTransferType.Expense ? t.Amount : 0,
                        TransferDate = t.TransferDate
                    })
                    .ToList();

                TurnoverListView.ItemsSource = filteredTransfers;
                TurnoverListView.Visibility = Visibility.Visible;
                viewListHeader.Visibility = Visibility.Visible;
                totalSummary.Visibility = Visibility.Visible;

                // Jami kirim va chiqimni hisoblash
                decimal totalIncome = filteredTransfers.Sum(t => t.Income);
                decimal totalExpense = filteredTransfers.Sum(t => t.Expense);

                // TextBlock’larni yangilash
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
    }
}