using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.KursDollar;

namespace DeLong_Desktop.Windows.DollarKurs;

/// <summary>
/// Interaction logic for DollarKursWindow.xaml
/// </summary>
public partial class DollarKursWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IKursDollarService kursDollarService;
    public DollarKursWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.kursDollarService = services.GetRequiredService<IKursDollarService>();
    }

    private void tbDollarKurs_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAndCleanInput(sender);
    }

    private async void btnAddDollarKurs_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (tbDollarKurs.Text.Equals(null)|| tbDollarOlishKurs.Text.Equals(null))
            {
                MessageBox.Show("Dollar kursini kiriting iltimos.");
                return;
            }

            KursDollarCreationDto kursDollarCreationDto = new KursDollarCreationDto()
            {
                SellingDollar = decimal.Parse(tbDollarKurs.Text),
                AdmissionDollar = decimal.Parse(tbDollarOlishKurs.Text)
            };

            // Dollar kursini qo'shish uchun xizmat chaqirilyapti
            bool result = await kursDollarService.AddAsync(kursDollarCreationDto);

            if (!result)
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

    private void tbDollarOlishKurs_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAndCleanInput(sender);
    }

    private void ValidateAndCleanInput(object sender)
    {
        if (sender is TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;

            // Kiritilgan matn
            string input = textBox.Text;

            // Tozalangan matn (faqat raqam va bitta nuqta)
            string cleanInput = "";
            bool hasDot = false;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    cleanInput += c;
                }
                else if (c == '.' && !hasDot)
                {
                    cleanInput += c;
                    hasDot = true;
                }
            }

            // Agar matn o'zgarsa, yangilaymiz
            if (textBox.Text != cleanInput)
            {
                textBox.Text = cleanInput;
                textBox.CaretIndex = Math.Min(caretIndex, cleanInput.Length);
            }
        }
    }
}
