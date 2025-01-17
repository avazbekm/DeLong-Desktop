using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

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

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        PriceUpdateDto priceUpdateDto = new PriceUpdateDto();

        // Yangi miqdorni saqlash logikasi
        MessageBox.Show($"Miqdori: {tbQuantity.Text}", "Ma'lumot");
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
