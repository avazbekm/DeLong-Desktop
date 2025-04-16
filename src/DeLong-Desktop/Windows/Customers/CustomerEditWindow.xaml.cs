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
/// Interaction logic for CustomerEditWindow.xaml
/// </summary>
public partial class CustomerEditWindow : Window
{
    private readonly ICustomerService customerService;
    private readonly IServiceProvider services;
    private readonly long customerId;
    public bool IsModified { get; set; } = false;

    public CustomerEditWindow(IServiceProvider services, long customerId)
    {
        InitializeComponent();
        this.services = services;
        this.customerService = services.GetRequiredService<ICustomerService>();
        this.customerId = customerId;

        LoadCustomerDataAsync();
    }

    // Mijoz ma'lumotlarini yuklash
    private async void LoadCustomerDataAsync()
    {
        try
        {
            var customer = await customerService.RetrieveByIdAsync(customerId);
            if (customer != null)
            {
                txtbInn.Text = customer.INN?.ToString() ?? string.Empty;
                txtbCompanyName.Text = customer.CompanyName.ToUpper();
                txtbManagerName.Text = customer.ManagerName.ToUpper();
                txtbManagerPhone.Text = customer.ManagerPhone;
                txtbMFO.Text = customer.MFO;
                txtbBankAccount.Text = customer.BankAccount;
                txtbBankName.Text = customer.BankName.ToUpper();
                txtbOknx.Text = customer.OKONX;
                txtbYurAddress.Text = customer.YurAddress.ToUpper();
                txtbEmployeeName.Text = customer.EmployeeName.ToUpper();
                txtbEmployeePhone.Text = customer.EmployeePhone;
                CustomerEditBranchId.UpdateBranchId = customer.BranchId;
            }
            else
            {
                MessageBox.Show("Mijoz ma'lumotlari topilmadi!", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mijoz ma'lumotlarini yuklashda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    // Yangilash tugmasi
    private async void btnYangilash_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // CustomerUpdateDto yaratish
            var updateDto = new CustomerUpdateDto
            {
                Id = customerId,
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
                BranchId = CustomerEditBranchId.UpdateBranchId
            };

            // Majburiy maydonlarni tekshirish
            if (string.IsNullOrWhiteSpace(updateDto.CompanyName))
            {
                MessageBox.Show("Kompaniya nomini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbCompanyName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(updateDto.ManagerName))
            {
                MessageBox.Show("Rahbar ismini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(updateDto.ManagerPhone))
            {
                MessageBox.Show("Rahbar telefon raqamini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerPhone.Focus();
                return;
            }

            // Telefon raqamlarni validatsiya qilish
            if (!string.IsNullOrWhiteSpace(updateDto.ManagerPhone) && !ValidationHelper.IsValidPhone(updateDto.ManagerPhone))
            {
                MessageBox.Show("Rahbar telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbManagerPhone.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.EmployeePhone) && !ValidationHelper.IsValidPhone(updateDto.EmployeePhone))
            {
                MessageBox.Show("Xodim telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbEmployeePhone.Focus();
                return;
            }

            // INN ni validatsiya qilish (agar kiritilgan bo‘lsa)
            if (updateDto.INN.HasValue && updateDto.INN.ToString().Length != 9)
            {
                MessageBox.Show("INN 9 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbInn.Focus();
                return;
            }

            // MFO ni validatsiya qilish (agar kiritilgan bo‘lsa)
            if (!string.IsNullOrWhiteSpace(updateDto.MFO) && updateDto.MFO.Length != 5)
            {
                MessageBox.Show("MFO 5 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbMFO.Focus();
                return;
            }

            // Bank hisob raqami (agar kiritilgan bo‘lsa)
            if (!string.IsNullOrWhiteSpace(updateDto.BankAccount) && updateDto.BankAccount.Length != 24)
            {
                MessageBox.Show("Bank hisob raqami 24 raqamdan iborat bo‘lishi kerak!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtbBankAccount.Focus();
                return;
            }

            // INN dublikatini tekshirish
            if (updateDto.INN.HasValue)
            {
                var existingCustomer = await customerService.RetrieveByInnAsync(updateDto.INN.Value);
                if (existingCustomer != null && existingCustomer.Id != customerId)
                {
                    MessageBox.Show($"Bu INN ({updateDto.INN.Value}) bilan boshqa mijoz allaqachon mavjud!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtbInn.Focus();
                    return;
                }
            }

            // Serverga yangilash
            var result = await customerService.ModifyAsync(updateDto);
            if (result)
            {
                MessageBox.Show("Mijoz muvaffaqiyatli yangilandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                IsModified = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Mijozni yangilashda xatolik yuz berdi!", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
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

    // Qaytish tugmasi
    private void btnQaytish_Click(object sender, RoutedEventArgs e)
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