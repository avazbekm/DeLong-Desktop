using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.Pages.Warehouses;
using DeLong_Desktop.Pages.SaleHistory;
using DeLong_Desktop.Pages.SalesPractice;
using DeLong_Desktop.Pages.AdditionalOperations;

namespace DeLong_Desktop;

/// <summary>
/// MainWindow.xaml bilan o'zaro aloqani boshqaradi.
/// </summary>
public partial class MainWindow : Window
{
    private CustomersPage _customerPage;
    private readonly IServiceProvider _services;

    // Tanlangan tilni saqlash uchun o'zgaruvchi
    private string _currentLanguage = "en";

    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        // Dastur boshlanganda tanlangan tilni sozlash
        SetLanguage(_currentLanguage);
    }

    /// <summary>
    /// Tilni o'zgartirish va interfeysni moslashtirish
    /// </summary>
    private void LanguageAPP(object sender, SelectionChangedEventArgs e)
    {
        if (languageComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedLanguage = selectedItem.Tag?.ToString();

            if (!string.IsNullOrEmpty(selectedLanguage) && _currentLanguage != selectedLanguage)
            {
                _currentLanguage = selectedLanguage;
                SetLanguage(_currentLanguage);
                UpdateLanguage(); // Interfeys matnlarini yangilash
            }
        }
    }

    /// <summary>
    /// Til sozlamalarini o'rnatadi.
    /// </summary>
    /// <param name="language">ISO formatida til kodi (masalan, "en", "uz-UZ")</param>
    private void SetLanguage(string language)
    {
        DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(language);
    }

    /// <summary>
    /// Interfeys va sahifa matnlarini yangilash.
    /// </summary>
    private void UpdateLanguage()
    {
        if (_customerPage == null)
        {
            _customerPage = new CustomersPage(_services);
        }

        _customerPage.userDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Customer;
        _customerPage.userDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.ClientFullname;
        _customerPage.userDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Phone;
        _customerPage.userDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Telegram_Phone_Number;
        _customerPage.userDataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.JSHSHIR;
        _customerPage.userDataGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Address;
        _customerPage.userDataGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.Action;
    md: HintAssist.SetHint(_customerPage.txtSearch, DeLong_Desktop.Resources.Resource.Search); // Hint ni yangilash
        _customerPage.btnAdd.Content = DeLong_Desktop.Resources.Resource.Add;
        _customerPage.btnExcel.Content = DeLong_Desktop.Resources.Resource.ToExcell;

        btnOmbor.Content = DeLong_Desktop.Resources.Resource.Warehouse;
        btnChiqim.Content = DeLong_Desktop.Resources.Resource.Expense;
        btnChiqish.Content = DeLong_Desktop.Resources.Resource.Exit;
        btnHisobot.Content = DeLong_Desktop.Resources.Resource.Report;
        btnMaxsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        btnMijoz.Content = DeLong_Desktop.Resources.Resource.Customer;
        btnKirim.Content = DeLong_Desktop.Resources.Resource.Income;
        btnSaleHistory.Content = DeLong_Desktop.Resources.Resource.SaleHistory;
        btnAdditionalOperations.Content = DeLong_Desktop.Resources.Resource.AdditionalOperations;
    }

    /// <summary>
    /// Mijozlar sahifasiga o'tish.
    /// </summary>
    private void bntMijoz_Click(object sender, RoutedEventArgs e)
    {
        if (_customerPage == null)
        {
            _customerPage = new CustomersPage(_services);
        }

        Navigator.Navigate(_customerPage);
        UpdateLanguage(); // Matnlarni sahifaga moslashtirish
    }

    /// <summary>
    /// Dasturni yopish tugmasi.
    /// </summary>
    private void btnChiqish_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Mahsulotlar sahifasiga o'tish.
    /// </summary>
    private void btnMaxsulot_Click(object sender, RoutedEventArgs e)
    {
        var productsPage = new ProductsPage(_services);
        Navigator.Navigate(productsPage);
    }

    /// <summary>
    /// Ombor sahifasiga o'tish.
    /// </summary>
    private void btnOmbor_Click(object sender, RoutedEventArgs e)
    {
        var warehousePage = new WarehousePage(_services);
        Navigator.Navigate(warehousePage);
    }

    /// <summary>
    /// Kirim sahifasiga o'tish.
    /// </summary>
    private void btnKirim_Click(object sender, RoutedEventArgs e)
    {
        var inputPage = new InputPage(_services);
        Navigator.Navigate(inputPage);
    }

    private void languageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // ComboBoxda tanlangan elementni olish
        if (languageComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            // Tanlangan til kodini ComboBox elementining Tag qiymatidan olish
            string selectedLanguage = selectedItem.Tag?.ToString();

            // Agar tanlangan til avvalgi tildan farqli bo'lsa
            if (!string.IsNullOrEmpty(selectedLanguage) && _currentLanguage != selectedLanguage)
            {
                // Hozirgi tanlangan tilni yangilash
                _currentLanguage = selectedLanguage;

                // Til sozlamalarini o'rnatish
                SetLanguage(_currentLanguage);

                // Interfeys matnlarini yangilash
                UpdateLanguage();
            }
        }
    }

    private void btnChiqim_Click(object sender, RoutedEventArgs e)
    {
        SalePracticePage salePracticePage = new SalePracticePage(_services);
        Navigator.Navigate(salePracticePage);
    }

    private void btnSaleHistory_Click(object sender, RoutedEventArgs e)
    {
        var saleHistoryPage = new SaleHistoryPage(_services);
        Navigator.Navigate(saleHistoryPage);
    }

    private void OnAdditionalOperationsClick(object sender, RoutedEventArgs e)
    {
        var additionalOperationsPage = new AdditionalOperationsPage(_services);
        Navigator.Navigate(additionalOperationsPage);
    }
}