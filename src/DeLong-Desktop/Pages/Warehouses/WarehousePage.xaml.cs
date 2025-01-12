using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using System.Windows.Data;
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

    private void btnExcel_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        // DataGrid ma'lumotlarini DataTable ga o'girish
        DataTable dataTable = new DataTable();

        foreach (var column in warehouseDataGrid.Columns)
        {
            dataTable.Columns.Add(column.Header.ToString());
        }

        foreach (var item in warehouseDataGrid.Items)
        {
            DataRow row = dataTable.NewRow();
            foreach (var column in warehouseDataGrid.Columns)
            {
                if (column is DataGridTextColumn textColumn)
                {
                    Binding binding = textColumn.Binding as Binding;
                    string propertyName = binding?.Path.Path;

                    if (propertyName != null && item != null)
                    {
                        var value = item.GetType().GetProperty(propertyName)?.GetValue(item, null);
                        row[column.Header.ToString()] = value ?? "";
                    }
                }
            }
            dataTable.Rows.Add(row);
        }

        // Excelga yozish
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Exported Data");
            worksheet.Cell(1, 1).InsertTable(dataTable);

            // Fayl saqlash dialog oynasi
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Fayl (*.xlsx)|*.xlsx",
                FileName = "Omborlar ro'yxati.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excelga o'tkazildi!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
