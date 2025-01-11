using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
                    Address = warehouse.Location
                });
        }
        userDataGrid.ItemsSource = items; ;
    }

}
