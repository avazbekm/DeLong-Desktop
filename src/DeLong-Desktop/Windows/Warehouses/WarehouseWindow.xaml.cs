using System.Windows;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Warehouses;

namespace DeLong_Desktop.Windows.Warehouses;

/// <summary>
/// Interaction logic for WarehouseWindow.xaml
/// </summary>
public partial class WarehouseWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IWarehouseService warehouseService;
    public WarehouseWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        warehouseService = services.GetRequiredService<IWarehouseService>();
    }

    private async void btnAddWarehouse_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // malumotlar to'ldirilganligini tekshirib olamiz
            if (txtbName.Text.Equals("") || txtbMudir.Text.Equals("") || txtbAddress.Text.Equals(""))
            {
                MessageBox.Show("Ma'lumotni to'liq kiriting.");
                return;
            }
            // malumotlarni dtoga beramiz
            WarehouseCreationDto warehouseCreationDto = new WarehouseCreationDto
            {
                Name = txtbName.Text.ToLower(),
                ManagerName = txtbMudir.Text.ToLower(),
                Location = txtbAddress.Text.ToLower()
            };

            // Warehouseni qo'shish uchun xizmat chaqirilyapti
            bool result = await warehouseService.AddAsync(warehouseCreationDto);

            if (result)
            {
                MessageBox.Show("Ombor muvaffaqiyatli saqlandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Kutilmagan xatoliklar uchun
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
