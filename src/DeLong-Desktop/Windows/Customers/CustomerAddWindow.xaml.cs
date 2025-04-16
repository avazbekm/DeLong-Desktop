using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeLong_Desktop.ApiService.DTOs.Customers;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Customers;

/// <summary>
/// Interaction logic for CustomerAddWindow.xaml
/// </summary>
public partial class CustomerAddWindow : Window
{
    private readonly ICustomerService customerService;
    private readonly IServiceProvider services;
    public bool IsCreated { get; set; } = false;

    public CustomerAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.customerService = services.GetRequiredService<ICustomerService>();
    }

    // Saqlash tugmasi
    private async void btnYurAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // CustomerCreationDto yaratish
            var customerCreationDto = new CustomerCreationDto
            {
                CompanyName = txtbCompanyName.Text.ToLower()?.Trim() ?? string.Empty,
                ManagerName = txtbManagerName.Text.ToLower()?.Trim() ?? string.Empty,
                ManagerPhone = txtbManagerPhone.Text?.Trim() ?? string.Empty,
                MFO = txtbMFO.Text?.Trim() ?? string.Empty,
                INN = string.IsNullOrWhiteSpace(txtbInn.Text) ? null : int.Parse(txtbInn.Text.Trim()),
                BankAccount = txtbBankAccount.Text?.Trim() ?? string.Empty,
                BankName = txtbBankName.Text.ToLower()?.Trim() ?? string.Empty,
                OKONX = txtbOknx.Text?.Trim() ?? string.Empty,
                YurAddress = txtbYurAddress.Text.ToLower()?.Trim() ?? string.Empty,
                EmployeeName = txtbEmployeeName.Text.ToLower()?.Trim() ?? string.Empty,
                EmployeePhone = txtbEmployeePhone.Text?.Trim() ?? string.Empty,
            };

            // Majburiy maydonlarni tekshirish
            if (string.IsNullOrWhiteSpace(customerCreationDto.CompanyName))
            {
                MessageBox.Show("Kompaniya nomini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbCompanyName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(customerCreationDto.ManagerName))
            {
                MessageBox.Show("Rahbar ismini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(customerCreationDto.ManagerPhone))
            {
                MessageBox.Show("Rahbar telefon raqamini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerPhone.Focus();
                return;
            }

            // Telefon raqamlarni validatsiya qilish
            if (!string.IsNullOrWhiteSpace(customerCreationDto.ManagerPhone) && !ValidationHelper.IsValidPhone(customerCreationDto.ManagerPhone))
            {
                MessageBox.Show("Rahbar telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerPhone.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(customerCreationDto.EmployeePhone) && !ValidationHelper.IsValidPhone(customerCreationDto.EmployeePhone))
            {
                MessageBox.Show("Xodim telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbEmployeePhone.Focus();
                return;
            }

            // INN ni validatsiya qilish (agar kiritilgan bo‘lsa)
            if (customerCreationDto.INN.HasValue && customerCreationDto.INN.ToString().Length != 9)
            {
                MessageBox.Show("INN 9 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbInn.Focus();
                return;
            }

            // MFO ni validatsiya qilish (agar kiritilgan bo‘lsa)
            if (!string.IsNullOrWhiteSpace(customerCreationDto.MFO) && customerCreationDto.MFO.Length != 5)
            {
                MessageBox.Show("MFO 5 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbMFO.Focus();
                return;
            }

            // Bank hisob raqami (agar kiritilgan bo‘lsa)
            if (!string.IsNullOrWhiteSpace(customerCreationDto.BankAccount) && customerCreationDto.BankAccount.Length != 24)
            {
                MessageBox.Show("Bank hisob raqami 24 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbBankAccount.Focus();
                return;
            }

            // INN dublikatini tekshirish
            if (customerCreationDto.INN.HasValue)
            {
                var existingCustomer = await customerService.RetrieveByInnAsync(customerCreationDto.INN.Value);
                if (existingCustomer != null)
                {
                    MessageBox.Show($"Bu INN ({customerCreationDto.INN.Value}) bilan mijoz allaqachon mavjud!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtbInn.Focus();
                    return;
                }
            }

            // Serverga yuborish
            var result = await customerService.AddAsync(customerCreationDto);
            if (result != null)
            {
                MessageBox.Show("Mijoz muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCreated = true;
                Close();
            }
            else
            {
                MessageBox.Show("Mijozni saqlashda xatolik yuz berdi!", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (FormatException)
        {
            MessageBox.Show("INN, MFO yoki hisob raqamiga faqat raqam kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Telefon raqamlarini formatlash
    private void FormatPhoneNumber(TextBox textBox)
    {
        if (textBox == null) return;

        string text = textBox.Text?.Trim() ?? string.Empty;
        string digits = Regex.Replace(text, @"[^\d]", ""); // Faqat raqamlarni olish

        // Hodisani vaqtincha o‘chirish
        textBox.TextChanged -= txtbManagerPhone_TextChanged;
        textBox.TextChanged -= txtbEmployeePhone_TextChanged;

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
            textBox.TextChanged += txtbManagerPhone_TextChanged;
            textBox.TextChanged += txtbEmployeePhone_TextChanged;
        }
    }

    // Manager Phone uchun TextChanged
    private void txtbManagerPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        FormatPhoneNumber(sender as TextBox);
        ValidationHelper.ValidatePhone(sender as TextBox);
    }

    // Employee Phone uchun TextChanged
    private void txtbEmployeePhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        FormatPhoneNumber(sender as TextBox);
        ValidationHelper.ValidatePhone(sender as TextBox);
    }

    // Telefon raqamlariga faqat raqam kiritish
    private void txtbManagerPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"[\d]");
    }

    private void txtbEmployeePhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"[\d]");
    }

    // Validatsiya hodisalari
    private void txtbInn_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void txtbMFO_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void txtbBankAccount_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateAccountNumber(sender as TextBox);
    }

    private void txtbOknx_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }
}