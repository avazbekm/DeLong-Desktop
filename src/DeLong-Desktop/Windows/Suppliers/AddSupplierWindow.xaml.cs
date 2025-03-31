using System.Windows;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Suppliers;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Suppliers;

/// <summary>
/// Interaction logic for AddSupplierWindow.xaml
/// </summary>
public partial class AddSupplierWindow : Window
{
    private readonly ISupplierService _supplierService;
    private readonly IServiceProvider services;
    public AddSupplierWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _supplierService = services.GetRequiredService<ISupplierService>();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        string name = txtName.Text.Trim();
        string phone = txtPhone.Text.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
        {
            MessageBox.Show("Iltimos, barcha maydonlarni to‘ldiring!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var suppliers = await _supplierService.RetrieveAllAsync();
            if (suppliers.Any(s => s.ContactInfo == phone))
            {
                MessageBox.Show("Bu telefon raqami allaqachon mavjud!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dto = new SupplierCreationDto
            {
                Name = name,
                ContactInfo = phone
            };

            var result = await _supplierService.AddAsync(dto);
            if (result != null)
            {
                MessageBox.Show("Taminotchi muvaffaqiyatli qo‘shildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
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
        DialogResult = false;
        Close();
    }
}
