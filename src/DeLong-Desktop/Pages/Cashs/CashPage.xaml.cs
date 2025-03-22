using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.Enums;

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
            LoadData();
            LoadWarehouseData();
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
            // 1. FromComboBox va ToComboBox bir xil bo'lmasligini tekshirish
            string from = (FromComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string to = (ToComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if ((from == "Zaxiradan" && to == "Zaxiraga") || (from == "Kassadan" && to == "Kassaga"))
            {
                MessageBox.Show("Qayerdan va qayerga o'tkazish bir xil bo'lmasligi kerak!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. AmountTextBox qiymati 0 bo'lmasligini tekshirish
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Miqdor 0 dan katta bo'lishi kerak va to'g'ri kiritilishi lozim!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Valyutani aniqlash
            string currency = (CurrencyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 4. Balanslarni xizmatlardan olish
            decimal availableBalance = 0;
            var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
            var warehouse = await _cashWarehouseService.RetrieveByIdAsync();

            if (openRegisters == null || !openRegisters.Any())
            {
                MessageBox.Show("Ochiq kassa topilmadi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Birinchi ochiq kassani olamiz
            var firstRegister = openRegisters.First();

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

            // 5. Miqdor yetarli ekanligini tekshirish
            if (amount > availableBalance)
            {
                MessageBox.Show($"Yetarli mablag' yo'q! {currency} bo'yicha joriy balans: {availableBalance}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 6. O'tkazma amalga oshirish (xizmat orqali)
            try
            {
                await PerformTransfer(from, to, currency, amount,  firstRegister.Id);
                MessageBox.Show($"O'tkazma muvaffaqiyatli: {amount} {currency} {from} dan {to} ga.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ma'lumotlarni qayta yuklash
                LoadData();
                LoadWarehouseData();

                // Maydonlarni tozalash
                AmountTextBox.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // O'tkazmani amalga oshirish va balanslarni yangilash uchun funksiya
        private async Task PerformTransfer(string from, string to, string currency, decimal amount, long cashRegisterId)
        {
            // 1. Transfer DTO yaratish
            var transferDto = new CashTransferCreationDto
            {
                CashRegisterId = cashRegisterId,
                From = from == "Zaxiradan" ? "Reserve" : "Cash",
                To = to == "Zaxiraga" ? "Reserve" : "Cash",
                Currency = currency,
                Amount = amount,
                Note = from == "Zaxiradan" ? "kassaga o'tkazildi":"zaxiraga o'tkazildi",
                TransferDate = DateTimeOffset.UtcNow,
                TransferType = (from == "Zaxiradan" && to == "Kassaga") ? CashTransferType.Income : CashTransferType.Expense // Yangi qo‘shildi
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
    }
}