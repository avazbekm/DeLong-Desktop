using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;
using DeLong_Desktop.ApiService.Services;

namespace DeLong_Desktop.Windows.DollarKurs
{
    public partial class DollarSellWindow : Window
    {
        private readonly IServiceProvider services;
        private readonly IKursDollarService kursDollarService;
        private readonly ICashRegisterService cashRegisterService;
        private readonly ICashTransferService cashTransferService;

        public DollarSellWindow(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
            this.kursDollarService = services.GetRequiredService<IKursDollarService>();
            this.cashRegisterService = services.GetRequiredService<ICashRegisterService>();
            this.cashTransferService = services.GetRequiredService<ICashTransferService>();
            tbDollarSotishKurs.Focus();
        }

        private async void btnSellDollar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Kassadagi ochiq registrlarni olish
                var cashRegisters = await cashRegisterService.RetrieveOpenRegistersAsync();
                if (cashRegisters == null || !cashRegisters.Any())
                {
                    MessageBox.Show("Hozirda ochiq kassalar yo‘q!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Oxirgi ochiq kassani olish
                var lastRegister = cashRegisters.LastOrDefault();
                decimal dollarBalance = lastRegister.UsdBalance; // Dollar qoldig'i
                decimal somBalance = lastRegister.UzsBalance; // So'm qoldig'i

                // 2. tbDollarSotishKurs qiymatini olish
                if (!decimal.TryParse(tbDollarSotishKurs.Text, out decimal dollarAmount) || dollarAmount <= 0)
                {
                    MessageBox.Show("Dollar miqdori to‘g‘ri kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. tbSomBerishKurs qiymatini olish
                if (!decimal.TryParse(tbSomBerishKurs.Text, out decimal somAmount) || somAmount <= 0)
                {
                    MessageBox.Show("So‘m miqdori to‘g‘ri kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 4. Dollar qoldig'ini solishtirish
                if (dollarBalance < dollarAmount)
                {
                    MessageBox.Show($"Kassada yetarli dollar yo‘q! Joriy balans: {dollarBalance:N2} USD", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 5. Balansni yangilash
                var updatedRegister = new CashRegisterUpdateDto
                {
                    Id = lastRegister.Id,
                    UzsBalance = somBalance + somAmount, // So'mga qo'shish
                    UzpBalance = lastRegister.UzpBalance, // Plastik o'zgarmaydi
                    UsdBalance = dollarBalance - dollarAmount, // Dollardan ayirish
                    DebtAmount = lastRegister.DebtAmount
                };

                // 6. Yangilangan kassa ma'lumotlarini serverga yuborish
                await cashRegisterService.ModifyAsync(updatedRegister);

                var cashTransferSom = new CashTransferCreationDto
                {
                    CashRegisterId = lastRegister.Id,
                    Amount = somAmount,
                    Currency = "So'm",
                    Note = "valyuta sotuvidan tushum"
                };
                await cashTransferService.AddAsync(cashTransferSom);

                var cashTransferDollar = new CashTransferCreationDto
                {
                    CashRegisterId = lastRegister.Id,
                    Amount = dollarAmount,
                    Currency = "Dollar",
                    Note = "valyuta sotildi"
                };
                await cashTransferService.AddAsync(cashTransferDollar);

                // 7. Muvaffaqiyatli xabar va oynani yopish
                MessageBox.Show($"Dollar sotildi: {dollarAmount:N2} USD, {somAmount:N2} so‘m qo‘shildi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void tbDollarSotishKurs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
                if (tbDollarSotishKurs.Text.Length > 0)
                {
                    var dollarKurs = await this.kursDollarService.RetrieveByIdAsync();
                    tbSomBerishKurs.Text = (dollarKurs.SellingDollar * decimal.Parse(tbDollarSotishKurs.Text)).ToString("N2");
                }
                else
                    tbSomBerishKurs.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tbSomBerishKurs_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }

}