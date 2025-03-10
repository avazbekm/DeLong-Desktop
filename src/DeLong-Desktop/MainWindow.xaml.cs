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
    private SalePracticePage _salePracticePage;
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
        if(_salePracticePage== null)
        {
            _salePracticePage = new SalePracticePage(_services);
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

        _salePracticePage.btnsotuvniamalgaoshirish.Content = DeLong_Desktop.Resources.Resource.Sotuv;
        _salePracticePage.btnMahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.tbdollorkursi.Text = DeLong_Desktop.Resources.Resource.dollorkursi;
        _salePracticePage.btnDollarKurs.Content = DeLong_Desktop.Resources.Resource.dollorkursini_kiriting;
        _salePracticePage.btnMiqdori.Content = DeLong_Desktop.Resources.Resource.Miqdori;
        _salePracticePage.btnvalyutaayirboshlash.Content = DeLong_Desktop.Resources.Resource.Valyutani_ayirboshlash_;
        _salePracticePage.cbxProduct.Text = DeLong_Desktop.Resources.Resource.Mahsulotni_tanlang_;
        _salePracticePage.btnProductSell.Content = DeLong_Desktop.Resources.Resource.Add;
        _salePracticePage.btnBuyDollar.Content = DeLong_Desktop.Resources.Resource.Sotib_olish_;
        _salePracticePage.btnSellDollar.Content = DeLong_Desktop.Resources.Resource.Sotish;
        _salePracticePage.lblmahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.ProductGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.t_r;
        _salePracticePage.ProductGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Mahsulot_nomi_;
        _salePracticePage.ProductGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Narxi;
        _salePracticePage.ProductGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Miqdori;
        _salePracticePage.ProductGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.O_lchov_birligi_;
        _salePracticePage.ProductGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Umumiy_summasi_;
        _salePracticePage.ProductGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.O_chirish;
        _salePracticePage.lblmijoz.Content = DeLong_Desktop.Resources.Resource.Mijozlar_;
        _salePracticePage.cbxPayment.Text = DeLong_Desktop.Resources.Resource.Mijozni_tanlang_;
        _salePracticePage.lbljami.Content = DeLong_Desktop.Resources.Resource.Jami_summa_;
        _salePracticePage.lblnaqd.Content = DeLong_Desktop.Resources.Resource.Naqd;
        _salePracticePage.lblplastik.Content = DeLong_Desktop.Resources.Resource.Plastik;
        _salePracticePage.lbldollor.Content = DeLong_Desktop.Resources.Resource.Dollar;
        _salePracticePage.lblqarz.Content = DeLong_Desktop.Resources.Resource.Qarz;
        _salePracticePage.lblchegirma.Content = DeLong_Desktop.Resources.Resource.Chegirma;
        _salePracticePage.dpDueDate.Text = DeLong_Desktop.Resources.Resource.DueData;
        _salePracticePage.lbltolov.Content = DeLong_Desktop.Resources.Resource.To_lov_summasi_;
        _salePracticePage.btnFinishSale.Content = DeLong_Desktop.Resources.Resource.Yakunlash;





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
        if (_salePracticePage == null)
        {
            _salePracticePage = new SalePracticePage(_services);
        }

        Navigator.Navigate(_salePracticePage);
        UpdateLanguage(); // Matnlarni sahifaga moslashtirish
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