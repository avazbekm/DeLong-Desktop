using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.Pages.Warehouses;
using DeLong_Desktop.Pages.SaleHistory;
using DeLong_Desktop.Windows.DollarKurs;
using DeLong_Desktop.Pages.SalesPractice;
using DeLong_Desktop.Pages.AdditionalOperations;

namespace DeLong_Desktop;

/// <summary>
/// MainWindow.xaml bilan o'zaro aloqani boshqaradi.
/// </summary>
public partial class MainWindow : Window
{
    private DollarKursWindow _dollorkurswindow;
    private AdditionalOperationsPage _additionalOperationsPage;
    private CustomersPage _customerPage;
    private WarehousePage _warehousePage;
    private SalePracticePage _salePracticePage;
    private readonly IServiceProvider _services;
    private ProductsPage _productpage;
    private InputPage _inputPage;

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

        if (_salePracticePage == null)
        {
            _salePracticePage = new SalePracticePage(_services);
        }

        if (_productpage == null)
        {
            _productpage = new ProductsPage(_services);
        }

        if (_warehousePage == null)
        {
            _warehousePage = new WarehousePage(_services);
        }

        if (_inputPage == null)
        {
            _inputPage = new InputPage(_services);
        }

        if (_additionalOperationsPage == null)
        {
            _additionalOperationsPage = new AdditionalOperationsPage(_services);
        }

        #region CustomerPage
        _customerPage.userDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Customer;
        _customerPage.userDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.ClientFullname;
        _customerPage.userDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Phone;
        _customerPage.userDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Telegram_Phone_Number;
        _customerPage.userDataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.JSHSHIR;
        _customerPage.userDataGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Address;
        _customerPage.userDataGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.Action;
        HintAssist.SetHint(_customerPage.txtSearch, DeLong_Desktop.Resources.Resource.Search); // Hint ni yangilash
        _customerPage.btnAdd.Content = DeLong_Desktop.Resources.Resource.Add;
        _customerPage.btnExcel.Content = DeLong_Desktop.Resources.Resource.ToExcell;
        #endregion

        #region KirimPage
        HintAssist.SetHint(_inputPage.txtSearch, DeLong_Desktop.Resources.Resource.kategoriya_boyicha_qidiruv);
        HintAssist.SetHint(_inputPage.txtProductSearch, DeLong_Desktop.Resources.Resource.Mahsulotni_qidirish_);
        _inputPage.dtcategoriya.Header = DeLong_Desktop.Resources.Resource.Kategoriya;
        _inputPage.dtmahsulot.Header = DeLong_Desktop.Resources.Resource.Product;
        #endregion

        #region SalePracticePage
        _salePracticePage.lbSotuvPage.Content = DeLong_Desktop.Resources.Resource.Sotuvni_amalga_oshirish_;
        _salePracticePage.btnMahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.tbdollorkursi.Text = DeLong_Desktop.Resources.Resource.Dollar_kursi_;
        _salePracticePage.btnDollarKurs.Content = DeLong_Desktop.Resources.Resource.Dollar_kursini_kiriting_;
        _salePracticePage.btnMiqdori.Content = DeLong_Desktop.Resources.Resource.Miqdori_;
        _salePracticePage.lbChangeValyuta.Content = DeLong_Desktop.Resources.Resource.Valyutani_ayirboshlash_;
        HintAssist.SetHint(_salePracticePage.cbxProduct, DeLong_Desktop.Resources.Resource.Mahsulotni_tanlang_);
        _salePracticePage.btnProductSell.Content = DeLong_Desktop.Resources.Resource.Add;
        _salePracticePage.btnBuyDollar.Content = DeLong_Desktop.Resources.Resource.Sotib_olish_;
        _salePracticePage.btnSellDollar.Content = DeLong_Desktop.Resources.Resource.Sotish_;
        _salePracticePage.lblmahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.ProductGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.T_r_;
        _salePracticePage.ProductGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Mahsulot_nomi_;
        _salePracticePage.ProductGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Narxi_;
        _salePracticePage.ProductGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Miqdori_;
        _salePracticePage.ProductGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Olchov_birligi;
        _salePracticePage.ProductGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Umumiy_summasi_;
        _salePracticePage.ProductGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.Ochirish;

        HintAssist.SetHint(_salePracticePage.cbxPayment,DeLong_Desktop.Resources.Resource.Mijozni_tanlang_);
        _salePracticePage.lbljami.Content = DeLong_Desktop.Resources.Resource.Jami_summa_;
        _salePracticePage.lblnaqd.Content = DeLong_Desktop.Resources.Resource.Naqd_;
        _salePracticePage.lblplastik.Content = DeLong_Desktop.Resources.Resource.Plastik_;
        _salePracticePage.lbldollor.Content = DeLong_Desktop.Resources.Resource.Dollar;
        _salePracticePage.lblqarz.Content = DeLong_Desktop.Resources.Resource.Qarz;
        _salePracticePage.lblchegirma.Content = DeLong_Desktop.Resources.Resource.Chegirma_;
        _salePracticePage.dpDueDate.Text = DeLong_Desktop.Resources.Resource.DueData_;
        _salePracticePage.lbltolov.Content = DeLong_Desktop.Resources.Resource.Tolovsummasi;
        _salePracticePage.btnFinishSale.Content = DeLong_Desktop.Resources.Resource.Yakunlash_;
        #endregion

        #region ProductPage
        HintAssist.SetHint(_productpage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        HintAssist.SetHint(_productpage.cmbCategory, DeLong_Desktop.Resources.Resource.Category);
        _productpage.btnAdd.Content = DeLong_Desktop.Resources.Resource.Add;
        _productpage.btnExcel.Content = DeLong_Desktop.Resources.Resource.ToExcell;
        _productpage.dataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Mahsulot_nomi_;
        _productpage.dataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Ombordagi_miqdor_;
        _productpage.dataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Kategoriya;
        _productpage.dataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Faolligi_;
        _productpage.dataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Amallar_;
        #endregion

        #region WarehousePage
        HintAssist.SetHint(_warehousePage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        _warehousePage.btnAdd.Content = DeLong_Desktop.Resources.Resource.Add;
        _warehousePage.btnExcel.Content = DeLong_Desktop.Resources.Resource.ToExcell;
        _warehousePage.warehouseDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Ombor_nomi_;
        _warehousePage.warehouseDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Ombor_mudiri_;
        _warehousePage.warehouseDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Address;
        _warehousePage.warehouseDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Action;
        #endregion

        #region MainWindow
        btnOmbor.Content = DeLong_Desktop.Resources.Resource.Warehouse;
        btnChiqim.Content = DeLong_Desktop.Resources.Resource.Expense;
        btnChiqish.Content = DeLong_Desktop.Resources.Resource.Exit;
        btnHisobot.Content = DeLong_Desktop.Resources.Resource.Report;
        btnMaxsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        btnMijoz.Content = DeLong_Desktop.Resources.Resource.Customer;
        btnKirim.Content = DeLong_Desktop.Resources.Resource.Income;
        btnSaleHistory.Content = DeLong_Desktop.Resources.Resource.SaleHistory;
        btnAdditionalOperations.Content = DeLong_Desktop.Resources.Resource.AdditionalOperations;
        #endregion

        #region AdditionalOperationPage
        _additionalOperationsPage.additionalActions.Text = DeLong_Desktop.Resources.Resource.Qoshimcha_amallar;
        HintAssist.SetHint(_additionalOperationsPage.tbSearchDebt, DeLong_Desktop.Resources.Resource.Search);
        _additionalOperationsPage.tbqarznitolash.Text = DeLong_Desktop.Resources.Resource.Qarzni_tolash;
        _additionalOperationsPage.dtmijoznomi.Header = DeLong_Desktop.Resources.Resource.Mijoz_nomi;
        _additionalOperationsPage.dtqarzsummasi.Header = DeLong_Desktop.Resources.Resource.Qarz_summasi;
        _additionalOperationsPage.dttolashmuddati.Header = DeLong_Desktop.Resources.Resource.Tolash_muddati;
        _additionalOperationsPage.tbTotalDebtLabel.Text = DeLong_Desktop.Resources.Resource.Jami_qarz;
        _additionalOperationsPage.btnPayAllDebts.Content = DeLong_Desktop.Resources.Resource.Tolash;
        HintAssist.SetHint(_additionalOperationsPage.tbDollarPayment,DeLong_Desktop.Resources.Resource.Dollar);
        HintAssist.SetHint(_additionalOperationsPage.tbCardPayment,DeLong_Desktop.Resources.Resource.Plastik_);
        HintAssist.SetHint(_additionalOperationsPage.tbCashPayment,DeLong_Desktop.Resources.Resource.Naqd_);
        #endregion

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

        if (_customerPage.txtSearch != null)
        {
            HintAssist.SetHint(_customerPage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        }
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
        if (_productpage == null)
        {
            _productpage = new ProductsPage(_services);
        }
        Navigator.Navigate(_productpage);
        UpdateLanguage();
        if (_productpage.txtSearch != null)
        {
            HintAssist.SetHint(_productpage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
            HintAssist.SetHint(_productpage.cmbCategory, DeLong_Desktop.Resources.Resource.Category);
        }
    }

    /// <summary>
    /// Ombor sahifasiga o'tish.
    /// </summary>
    private void btnOmbor_Click(object sender, RoutedEventArgs e)
    {
        if (_warehousePage == null)
        {
            _warehousePage = new WarehousePage(_services);
        }

        Navigator.Navigate(_warehousePage);
        UpdateLanguage(); // Matnlarni sahifaga moslashtirish

        if (_warehousePage.txtSearch != null)
        {
            HintAssist.SetHint(_warehousePage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        }
    }

    /// <summary>
    /// Kirim sahifasiga o'tish.
    /// </summary>
    private void btnKirim_Click(object sender, RoutedEventArgs e)
    {
        if (_inputPage == null)
        {
            _inputPage = new InputPage(_services);
        }
        Navigator.Navigate(_inputPage);
        UpdateLanguage();
        if (_inputPage.txtSearch != null)
        {
            HintAssist.SetHint(_inputPage.txtSearch, DeLong_Desktop.Resources.Resource.kategoriya_boyicha_qidiruv);
        }
        if (_inputPage.txtProductSearch != null)
        {
            HintAssist.SetHint(_inputPage.txtProductSearch, DeLong_Desktop.Resources.Resource.Mahsulotni_qidirish_);
        }

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
        if (_additionalOperationsPage == null)
        {
            _additionalOperationsPage = new AdditionalOperationsPage(_services);
        }
        Navigator.Navigate(_additionalOperationsPage);
        UpdateLanguage();
        if (_additionalOperationsPage.tbSearchDebt != null)
        {
            HintAssist.SetHint(_additionalOperationsPage.tbSearchDebt, DeLong_Desktop.Resources.Resource.Search);
            HintAssist.SetHint(_additionalOperationsPage.tbDollarPayment, DeLong_Desktop.Resources.Resource.Dollar);
            HintAssist.SetHint(_additionalOperationsPage.tbCardPayment, DeLong_Desktop.Resources.Resource.Plastik_);
            HintAssist.SetHint(_additionalOperationsPage.tbCashPayment, DeLong_Desktop.Resources.Resource.Naqd_);
        }
    }
} 
