using DeLong_Desktop.Windows.Customers;
using System.Windows.Controls;
using System.Xml.XPath;

namespace DeLong_Desktop.Pages.Customers;

/// <summary>
/// Interaction logic for CustomersPage.xaml
/// </summary>
public partial class CustomersPage : Page
{
    private readonly IServiceProvider services;

    public CustomersPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CustomerAddWindow customerAddWindow = new CustomerAddWindow(services);
        customerAddWindow.ShowDialog();
    }
}
