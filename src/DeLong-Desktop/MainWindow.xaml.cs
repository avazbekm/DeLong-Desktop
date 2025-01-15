using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.Pages.Warehouses;

namespace DeLong_Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public CustomersPage _customerPage;
    private readonly IServiceProvider services;
    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }
    private string _currentLanguage = "en"; // Tanlangan tilni saqlash uchun o'zgaruvchi

    private void LanguageAPP(object sender, SelectionChangedEventArgs e)
    {
        if (languageComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedLanguage = selectedItem.Tag.ToString();

            if (_currentLanguage != selectedLanguage) // Faqat til o'zgarishida yangilash
            {
                _currentLanguage = selectedLanguage;
                DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(selectedLanguage);
                UpdateLanguage(); // Matnlarni yangilash
            }
        }
    }
    private void UpdateLanguage()
    {
        if (_customerPage == null)
        {
            _customerPage = new CustomersPage(services);
        }
        _customerPage.userDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Customer;


    }
    private void bntMijoz_Click(object sender, RoutedEventArgs e)
    {
        if (_customerPage == null)
        {
            _customerPage = new CustomersPage(services); // UserPage faqat bir marta yaratiladi
        }

        // Tanlangan tilni qo'llash
        DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(_currentLanguage); // Tanlangan tilni qo'llash
        UpdateLanguage(); // Matnlarni yangilash

        // UserPage sahifasiga o'tish
        Navigator.Navigate(_customerPage);
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

    private void btnKirim_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(new InputPage(services));
    }
}