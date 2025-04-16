using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeLong_Desktop.ApiService.DTOs.Suppliers;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Suppliers;

/// <summary>
/// Interaction logic for AddSupplierWindow.xaml
/// </summary>
public partial class AddSupplierWindow : Window
{
    private readonly ISupplierService _supplierService;
    private readonly IServiceProvider services;
    public bool IsCreated { get; set; } = false;

    public AddSupplierWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _supplierService = services.GetRequiredService<ISupplierService>();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        string name = txtName.Text?.Trim() ?? string.Empty;
        string phone = txtPhone.Text?.Trim() ?? string.Empty;

        // Majburiy maydonlarni tekshirish
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Iltimos, taminotchi nomini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            txtName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            MessageBox.Show("Iltimos, telefon raqamini kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            txtPhone.Focus();
            return;
        }

        // Telefon raqam formati tekshiruvi
        if (!ValidationHelper.IsValidPhone(phone))
        {
            MessageBox.Show("Telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            txtPhone.Focus();
            return;
        }

        try
        {
            // Telefon raqami dublikatini tekshirish
            var suppliers = await _supplierService.RetrieveAllAsync();
            if (suppliers.Any(s => s.ContactInfo == phone))
            {
                MessageBox.Show("Bu telefon raqami allaqachon mavjud!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPhone.Focus();
                return;
            }

            // DTO yaratish
            var dto = new SupplierCreationDto
            {
                Name = name.ToLower(),
                ContactInfo = phone
            };

            // Serverga yuborish
            var result = await _supplierService.AddAsync(dto);
            if (result != null)
            {
                MessageBox.Show("Taminotchi muvaffaqiyatli qo‘shildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCreated = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Taminotchi qo‘shishda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Telefon raqamlarini formatlash
    private void FormatPhoneNumber(TextBox textBox)
    {
        if (textBox == null) return;

        string text = textBox.Text?.Trim() ?? string.Empty;
        string digits = Regex.Replace(text, @"[^\d]", ""); // Faqat raqamlarni olish

        // Hodisani vaqtincha o‘chirish
        textBox.TextChanged -= txtPhone_TextChanged;

        try
        {
            // Agar bo‘sh bo‘lsa yoki faqat +998 kiritilgan bo‘lsa
            if (digits.Length <= 3)
            {
                textBox.Text = "+998 ";
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }

            // Raqamlarni formatlash
            string formatted = "+998 ";
            int remainingDigits = digits.Length - 3; // +998 dan keyingi raqamlar

            if (remainingDigits > 0)
            {
                // 2 raqamli kod
                formatted += digits.Substring(3, Math.Min(2, remainingDigits));
                if (remainingDigits > 2)
                {
                    formatted += " ";
                    // Qolgan 7 raqam
                    formatted += digits.Substring(5, Math.Min(7, remainingDigits - 2));
                }
            }

            textBox.Text = formatted.Trim();
            textBox.SelectionStart = textBox.Text.Length;
        }
        finally
        {
            // Hodisani qayta ulash
            textBox.TextChanged += txtPhone_TextChanged;
        }
    }

    // Telefon raqami uchun TextChanged
    private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        FormatPhoneNumber(sender as TextBox);
        ValidationHelper.ValidatePhone(sender as TextBox);
    }

    // Telefon raqamiga faqat raqam kiritish
    private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"[\d]");
    }
}