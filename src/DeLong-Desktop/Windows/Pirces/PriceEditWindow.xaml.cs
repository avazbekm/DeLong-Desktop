using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

/// <summary>
/// Interaction logic for PriceEditWindow.xaml
/// </summary>
public partial class PriceEditWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IPriceService priceService;
    public PriceEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        priceService = services.GetRequiredService<IPriceService>();
    }

    private void tbIncomePrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Save the cursor position
            int selectionStart = textBox.SelectionStart;

            // Filter out invalid characters and ensure only one decimal point
            string filteredText = string.Empty;
            bool decimalPointSeen = false;

            foreach (char c in textBox.Text)
            {
                if (char.IsDigit(c))
                {
                    filteredText += c;
                }
                else if (c == '.' && !decimalPointSeen)
                {
                    filteredText += c;
                    decimalPointSeen = true;
                }
            }

            // Update the TextBox content if the text was modified
            if (textBox.Text != filteredText)
            {
                textBox.Text = filteredText;

                // Restore the cursor position
                textBox.SelectionStart = Math.Min(selectionStart, filteredText.Length);
            }
        }
    }

    private void tbSellPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Save the cursor position
            int selectionStart = textBox.SelectionStart;

            // Filter out invalid characters and ensure only one decimal point
            string filteredText = string.Empty;
            bool decimalPointSeen = false;

            foreach (char c in textBox.Text)
            {
                if (char.IsDigit(c))
                {
                    filteredText += c;
                }
                else if (c == '.' && !decimalPointSeen)
                {
                    filteredText += c;
                    decimalPointSeen = true;
                }
            }

            // Update the TextBox content if the text was modified
            if (textBox.Text != filteredText)
            {
                textBox.Text = filteredText;

                // Restore the cursor position
                textBox.SelectionStart = Math.Min(selectionStart, filteredText.Length);
            }
        }
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Save the cursor position
            int selectionStart = textBox.SelectionStart;

            // Filter out invalid characters and ensure only one decimal point
            string filteredText = string.Empty;
            bool decimalPointSeen = false;

            foreach (char c in textBox.Text)
            {
                if (char.IsDigit(c))
                {
                    filteredText += c;
                }
                else if (c == '.' && !decimalPointSeen)
                {
                    filteredText += c;
                    decimalPointSeen = true;
                }
            }

            // Update the TextBox content if the text was modified
            if (textBox.Text != filteredText)
            {
                textBox.Text = filteredText;

                // Restore the cursor position
                textBox.SelectionStart = Math.Min(selectionStart, filteredText.Length);
            }
        }
    }

    private async void btnEditPrice_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            PriceUpdateDto priceUpdateDto = new PriceUpdateDto()
            {
                Id = PriceInfo.PriceId,
                CostPrice = decimal.Parse(tbIncomePrice.Text),
                SellingPrice = decimal.Parse(tbSellPrice.Text),
                Quantity = PriceInfo.Quatitiy,
                UnitOfMeasure = tbUnitOfMesure.Text,
                ProductId = InputInfo.ProductId
            };

            // Warehouseni qo'shish uchun xizmat chaqirilyapti
            bool result = await priceService.ModifyAsync(priceUpdateDto);

            if (result)
            {
                MessageBox.Show($"Narx muvaffaqiyatli yangilandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("O'zgarishda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Kutilmagan xatoliklar uchun
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Close(); // Yopish
    }
}
