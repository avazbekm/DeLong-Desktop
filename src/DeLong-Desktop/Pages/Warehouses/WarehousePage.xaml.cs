using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.Windows.Warehouses;
using Microsoft.Extensions.DependencyInjection;
using static MaterialDesignThemes.Wpf.Theme;

namespace DeLong_Desktop.Pages.Warehouses;

/// <summary>
/// Interaction logic for WarehousePage.xaml
/// </summary>
public partial class WarehousePage : Page
{
    private readonly IWarehouseService warehouseService;
    private readonly IServiceProvider services;
    public WarehousePage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        warehouseService = services.GetRequiredService<IWarehouseService>();
        Loading();
    }
    private async void Loading()
    {
        //datagridga malumotlar to'plash ushun
        List<Item> items = new List<Item>();

        // warehouselarni databasadan chaqirvodik
        var existWarehouses = await warehouseService.RetrieveAllAsync();

        if (existWarehouses is not null)
        {
            foreach(var warehouse in existWarehouses)
                items.Add(new Item()
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name.ToUpper(),
                    ManagerName = warehouse.ManagerName.ToUpper(),
                    Address = warehouse.Location.ToUpper(),
                });
        }
        warehouseDataGrid.ItemsSource = items; ;
    }

    private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        WarehouseWindow warehouseWindow = new WarehouseWindow(services);
        warehouseWindow.ShowDialog();
    }

    private async void btnEdit_Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        WarehouseEditWindow warehouseEditWindow = new WarehouseEditWindow(services);
        if (warehouseDataGrid.SelectedItem is Item selectedWarehouse)
        {
            WarehouseInfo.WarehouseId = selectedWarehouse.Id;
            warehouseEditWindow.txtbName.Text = selectedWarehouse.Name;
            warehouseEditWindow.txtbMudir.Text = selectedWarehouse.ManagerName;
            warehouseEditWindow.txtbAddress.Text = selectedWarehouse.Address;
        }

        warehouseEditWindow.ShowDialog();
    }
}
