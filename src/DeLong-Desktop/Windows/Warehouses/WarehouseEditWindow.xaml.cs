using System.Windows;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Warehouses;
using DeLong_Desktop.Pages.Warehouses;

namespace DeLong_Desktop.Windows.Warehouses;

/// <summary>
/// Interaction logic for WarehouseEditWindow.xaml
/// </summary>
public partial class WarehouseEditWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IWarehouseService warehouseService;
    public WarehouseEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        warehouseService = services.GetRequiredService<IWarehouseService>();
    }

    private async void btnUpdateWarehouse_Click(object sender, RoutedEventArgs e)
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
            WarehouseUpdatedDto warehouseUpdatedDto = new WarehouseUpdatedDto
            {
                Id = WarehouseInfo.WarehouseId,
                Name = txtbName.Text.ToLower(),
                ManagerName = txtbMudir.Text.ToLower(),
                Location = txtbAddress.Text.ToLower()
            };

            // Warehouseni qo'shish uchun xizmat chaqirilyapti
            bool result = await warehouseService.ModifyAsync(warehouseUpdatedDto);

            if (result)
            {
                MessageBox.Show("Ombor muvaffaqiyatli yangilandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
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
