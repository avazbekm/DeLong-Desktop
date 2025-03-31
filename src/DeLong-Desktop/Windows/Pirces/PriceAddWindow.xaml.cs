using System;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

public partial class PriceAddWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IPriceService priceService;
    public bool IsModified { get; set; } = false;
    public event EventHandler PriceModified; // Yangi hodisa

    public PriceAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        tbQuantity.Focus();
        this.services = services;
        priceService = services.GetRequiredService<IPriceService>();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(tbQuantity.Text))
            {
                MessageBox.Show("Miqdorni kiriting.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PriceUpdateDto priceUpdateDto = new PriceUpdateDto()
            {
                Id = PriceInfo.PriceId,
                CostPrice = PriceInfo.ArrivalPrice,
                SellingPrice = PriceInfo.SellingPrice,
                Quantity = PriceInfo.Quatitiy + decimal.Parse(tbQuantity.Text),
                UnitOfMeasure = PriceInfo.UnitOfMesure,
                ProductId = InputInfo.ProductId
            };

            bool result = await priceService.ModifyAsync(priceUpdateDto);

            if (result)
            {
                MessageBox.Show($"{tbQuantity.Text} {PriceInfo.UnitOfMesure} muvaffaqiyatli qo'shildi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                IsModified = true;
                PriceModified?.Invoke(this, EventArgs.Empty); // Yangilash signalini yuborish
                Close();
            }
            else
            {
                MessageBox.Show("Qo'shishda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null)
        {
            string currentText = textBox.Text;
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
                    hasDot = true;
                }
            }

            if (currentText != filteredText)
            {
                int caretIndex = textBox.CaretIndex - (currentText.Length - filteredText.Length);
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Max(0, caretIndex);
            }
        }
    }
}