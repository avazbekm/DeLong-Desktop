using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.Pages.Products;

namespace DeLong_Desktop.Pages.Reports
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private readonly IServiceProvider _services;
        private ProductWiewPage _productpage;
        public ReportPage(IServiceProvider services)
        {
            InitializeComponent();
            _services = services; // Xizmatlarni saqlaymiz
        }
        private void ProductList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProductWiewPage(_services));
        }

        private void SupplierList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CustomerList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void SalesReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void PurchasesReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Profit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void StockBalance_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
