using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;

namespace DeLong_Desktop.Pages.Cashs;

/// <summary>
/// Interaction logic for CashPage.xaml
/// </summary>
public partial class CashPage : Page
{
    private readonly ICashRegisterService _cashRegisterService;
    private readonly ICashTransferService _cashTransferService;
    private readonly ICashWarehouseService _cashWarehouseService;

    public CashPage(IServiceProvider services)
    {
        InitializeComponent();
        _cashRegisterService = services.GetRequiredService<ICashRegisterService>();
        _cashTransferService = services.GetRequiredService<ICashTransferService>();
        _cashWarehouseService = services.GetRequiredService<ICashWarehouseService>();
        LoadData();
        LoadWarehouseData();
    }

    private async void LoadWarehouseData()
    {
        var latestWarehouse = await _cashWarehouseService.RetrieveByIdAsync();
        CashWarehouseGrid.ItemsSource = new List<CashWarehouseResultDto> { latestWarehouse }; // ItemsControl uchun ro‘yxat
    }
    private async void LoadData()
    {
        // Faqat ochiq kassalarni olish
        var openRegisters = await _cashRegisterService.RetrieveOpenRegistersAsync();
        CashRegisterGrid.ItemsSource = openRegisters;

        if (openRegisters == null || !openRegisters.Any())
        {
            System.Windows.MessageBox.Show("Hozirda ochiq kassalar yo‘q!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }

    private void AddCashTransfer_Click(object sender, RoutedEventArgs e)
    {

    }

    private void AddWarehouse_Click(object sender, RoutedEventArgs e)
    {

    }

    private void AddCashRegister_Click(object sender, RoutedEventArgs e)
    {

    }

    private void NoteTextBox_GotFocus(object sender, RoutedEventArgs e)
    {

    }

    private void TransferButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void AmountTextBox_GotFocus(object sender, RoutedEventArgs e)
    {

    }

    private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
    {

    }

    private void NoteTextBox_LostFocus(object sender, RoutedEventArgs e)
    {

    }

    private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
