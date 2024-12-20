using DeLong_Desktop.Pages.Customers;
using System.Windows;

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
}