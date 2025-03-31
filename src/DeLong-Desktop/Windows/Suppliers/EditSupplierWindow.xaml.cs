using DeLong_Desktop.ApiService.DTOs.Suppliers;
using DeLong_Desktop.ApiService.Interfaces;
using System.Windows;

namespace DeLong_Desktop.Windows.Suppliers;

/// <summary>
/// Interaction logic for EditSupplierWindow.xaml
/// </summary>
public partial class EditSupplierWindow : Window
{
    private readonly ISupplierService _supplierService;
    private readonly SupplierResultDto _supplier;

    public EditSupplierWindow(ISupplierService supplierService, SupplierResultDto supplier)
    {
        InitializeComponent();
        _supplierService = supplierService;
        _supplier = supplier;

        txtName.Text = supplier.Name;
        txtPhone.Text = supplier.ContactInfo;
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
            var dto = new SupplierUpdateDto
            {
                Id = _supplier.Id,
                Name = name,
                ContactInfo = phone
            };

            var result = await _supplierService.ModifyAsync(dto);
            if (result != null)
            {
                MessageBox.Show("Taminotchi muvaffaqiyatli yangilandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Taminotchi yangilashda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
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
