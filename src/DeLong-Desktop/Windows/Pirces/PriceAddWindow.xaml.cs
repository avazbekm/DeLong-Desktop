using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

/// <summary>
/// Interaction logic for PriceAddWindow.xaml
/// </summary>
public partial class PriceAddWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IPriceService priceService;

    public PriceAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        tbQuantity.Focus(); // Ochilganda fokusni o'rnatish
        this.services = services;
        priceService = services.GetRequiredService<IPriceService>();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            PriceUpdateDto priceUpdateDto = new PriceUpdateDto()
            {
                Id = PriceInfo.PriceId,
                CostPrice = PriceInfo.ArrivalPrice,
                SellingPrice = PriceInfo.SellingPrice,
                Quantity = PriceInfo.Quatitiy + decimal.Parse(tbQuantity.Text),
                UnitOfMeasure = PriceInfo.UnitOfMesure,
                ProductId = InputInfo.ProductId
            };

            // Warehouseni qo'shish uchun xizmat chaqirilyapti
            bool result = await priceService.ModifyAsync(priceUpdateDto);

            if (result)
            {
                MessageBox.Show($"{tbQuantity.Text} {PriceInfo.UnitOfMesure} muvaffaqiyatli qo'shildi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Qo'shishda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Kutilmagan xatoliklar uchun
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Close(); // Yopish
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;

        if (textBox != null)
        {
            // Hozirgi matnni olish
            string currentText = textBox.Text;

            // Faqat raqamlar va bir dona "." ga ruxsat berish
            string filteredText = "";
            bool hasDot = false;

            foreach (char c in currentText)
            {
                if (char.IsDigit(c))
                {
                    filteredText += c;
                }
                else if (c == '.' && !hasDot)
                {
                    filteredText += c;
                    hasDot = true; // "." faqat bir marta bo'lishi uchun
                }
            }

            // Matnni yangilash, agar o'zgartirish kerak bo'lsa
            if (currentText != filteredText)
            {
                int caretIndex = textBox.CaretIndex - (currentText.Length - filteredText.Length);
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Max(0, caretIndex); // Kursorni saqlash
            }
        }
    }

}
