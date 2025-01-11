using System.Windows;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.Pages.Warehouses;

namespace DeLong_Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IServiceProvider services;
    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private void bntMijoz_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to CustomerPage
        Navigator.Navigate(new CustomersPage(services));
    }

    private void btnChiqish_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnMaxsulot_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(new ProductsPage(services));
    }

    private void btnOmbor_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(new WarehousePage(services));
    }
}