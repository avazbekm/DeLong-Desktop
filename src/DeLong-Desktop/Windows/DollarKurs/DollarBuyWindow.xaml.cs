using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;

namespace DeLong_Desktop.Windows.DollarKurs
{
    public partial class DollarBuyWindow : Window
    {
        private readonly IServiceProvider services;
        private readonly IKursDollarService kursDollarService;
        private readonly ICashRegisterService cashRegisterService;
        private readonly ICashTransferService cashTransferService;

        public DollarBuyWindow(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
            this.kursDollarService = services.GetRequiredService<IKursDollarService>();
            this.cashRegisterService = services.GetRequiredService<ICashRegisterService>();
            this.cashTransferService = services.GetRequiredService<ICashTransferService>();
            tbDollarKurs.Focus();
        }

        private async void btnAddDollarKurs_Click(object sender, RoutedEventArgs e)
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
                decimal somBalance = lastRegister.UzsBalance; // So'm qoldig'i
                decimal dollarBalance = lastRegister.UsdBalance; // Dollar qoldig'i

                // 2. tbDollarOlishKurs qiymatini olish
                if (!decimal.TryParse(tbDollarOlishKurs.Text, out decimal somAmount) || somAmount <= 0)
                {
                    MessageBox.Show("So‘m miqdori to‘g‘ri kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. tbDollarKurs qiymatini olish
                if (!decimal.TryParse(tbDollarKurs.Text, out decimal dollarAmount) || dollarAmount <= 0)
                {
                    MessageBox.Show("Dollar miqdori to‘g‘ri kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 4. So'm qoldig'ini solishtirish
                if (somBalance < somAmount)
                {
                    MessageBox.Show($"Kassada yetarli so‘m yo‘q! Joriy balans: {somBalance:N2} so‘m", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 5. Balansni yangilash
                var updatedRegister = new CashRegisterUpdateDto
                {
                    Id = lastRegister.Id,
                    UzsBalance = somBalance - somAmount, // So'mdan ayirish
                    UzpBalance = lastRegister.UzpBalance, // Plastik o'zgarmaydi
                    UsdBalance = dollarBalance + dollarAmount, // Dollarga qo'shish
                    DebtAmount = lastRegister.DebtAmount
                };

                // 6. Yangilangan kassa ma'lumotlarini serverga yuborish
                await cashRegisterService.ModifyAsync(updatedRegister);
        
                var cashTransferSom = new CashTransferCreationDto
                {
                    Amount = somAmount,
                    CashRegisterId = lastRegister.Id,
                    Currency = "So'm",
                    Note = "valyuta sotib olish uchun"
                };
                await cashTransferService.AddAsync(cashTransferSom);

                var cashTransferDollar = new CashTransferCreationDto
                {
                    CashRegisterId = lastRegister.Id,
                    Amount = dollarAmount,
                    Currency = "Dollar",
                    Note = "valyuta sotib olindi"
                };
                await cashTransferService.AddAsync(cashTransferDollar);


                // 7. Muvaffaqiyatli xabar va oynani yopish
                MessageBox.Show($"Dollar sotib olindi: {dollarAmount:N2} USD, {somAmount:N2} so‘m ayirildi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void tbDollarKurs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
                if (tbDollarKurs.Text.Length > 0)
                {
                    var dollarKurs = await this.kursDollarService.RetrieveByIdAsync();
                    tbDollarOlishKurs.Text = (decimal.Parse(tbDollarKurs.Text) * dollarKurs.AdmissionDollar).ToString("N2");
                }
                else
                    tbDollarOlishKurs.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tbDollarOlishKurs_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }

    // Taxminiy DTO'lar
}