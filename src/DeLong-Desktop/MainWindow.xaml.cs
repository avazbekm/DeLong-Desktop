using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Pages.Cashs;
using DeLong_Desktop.Pages.Reports;
using DeLong_Desktop.Pages.Branches;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.Pages.Suppliers;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.Pages.Languages;
using DeLong_Desktop.Pages.SaleHistory;
using DeLong_Desktop.Pages.SalesPractice;
using DeLong_Desktop.Pages.AdditionalOperations;
using DeLong_Desktop.Pages.Employees;
using DeLong_Desktop.Pages.InputHistories;

namespace DeLong_Desktop;

/// <summary>
/// MainWindow.xaml bilan o'zaro aloqani boshqaradi.
/// </summary>
public partial class MainWindow : Window
{
    private CashPage _cashPage;
    private InputPage _inputPage;
    private ProductsPage _productpage;
    private CustomersPage _customerPage;
    private SaleHistoryPage _saleHistoryPage;
    private SalePracticePage _salePracticePage;
    private readonly IServiceProvider _services;
    private AdditionalOperationsPage _additionalOperationsPage;

    private string _currentLanguage = "en";

    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        _services = services ?? throw new ArgumentNullException(nameof(services));

        _cashPage = new CashPage(_services);
        _inputPage = new InputPage(_services);
        _productpage = new ProductsPage(_services);
        _customerPage = new CustomersPage(_services);
        _saleHistoryPage = new SaleHistoryPage(_services);
        _salePracticePage = new SalePracticePage(_services);
        _additionalOperationsPage = new AdditionalOperationsPage(_services);

        SetLanguage(_currentLanguage);
        UpdateLanguage();
    }

    // PreviewMouseLeftButtonDown hodisasi bilan navigatsiya
    private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is TreeViewItem selectedItem && selectedItem.Tag != null)
        {
            switch (selectedItem.Tag.ToString())
            {
                case "SalesOperations":
                    Navigator.Navigate(_salePracticePage);
                    break;

                case "SalesHistory":
                    Navigator.Navigate(_saleHistoryPage);
                    break;

                case "ProductsList":
                    Navigator.Navigate(_productpage);
                    break;

                case "Incoming":
                    Navigator.Navigate(_inputPage);
                    break;

                case "Suppliers":
                    Navigator.Navigate(new SuppliersPage(_services));
                    break;

                case "Customers":
                    Navigator.Navigate(_customerPage);
                    break;

                case "Languages": // Yangilangan qator
                    Navigator.Navigate(new LanguagePage(this)); // MainWindow'ni konstruktor sifatida uzatamiz
                    break;

                case "Branches":
                    Navigator.Navigate(new BranchesPage(_services));
                    break;

                case "Employees":
                    Navigator.Navigate(new EmployeesPage(_services));
                    break;

                case "History":
                    Navigator.Navigate(new HistoryPage(_services));
                    break;
            }

            UpdateLanguage();
            e.Handled = true;
        }
    }

    private void SetLanguage(string language)
    {
        DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(language);
    }

    private void UpdateLanguage()
    {
        #region CashPage
        _cashPage.tbkassaniboshqarish.Text = DeLong_Desktop.Resources.Resource.Kassani_boshqarish;
        _cashPage.OpenDayButton.Content = DeLong_Desktop.Resources.Resource.Kunni_ochish;
        _cashPage.tbkassaqoldiqlari.Text = DeLong_Desktop.Resources.Resource.Kassani_boshqarish;
        _cashPage.ShowTurnoverButton.Content = DeLong_Desktop.Resources.Resource.Kassa_aylanmasi;
        _cashPage.tbkassaotkazmalari.Text = DeLong_Desktop.Resources.Resource.Kassa_o_tkazmalari;
        _cashPage.TransferButton.Content = DeLong_Desktop.Resources.Resource.O_tkazish;
        _cashPage.tbzaxiradagipullar.Text = DeLong_Desktop.Resources.Resource.Zaxiradagi_pullar;
        HintAssist.SetHint(_cashPage.NoteTextBox, DeLong_Desktop.Resources.Resource.Izoh);
        #endregion

        #region CustomerPage
        _customerPage.userDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.CustomerName;
        _customerPage.userDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.EmployeeFullname;
        _customerPage.userDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.EmployeePhone;
        _customerPage.userDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.ManagerFullName;
        _customerPage.userDataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.EmployeePhone;
        _customerPage.userDataGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Address;
        _customerPage.userDataGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.Action;
        HintAssist.SetHint(_customerPage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        _customerPage.btnAdd.Content = DeLong_Desktop.Resources.Resource.Add;
        _customerPage.btnExcel.Content = DeLong_Desktop.Resources.Resource.ToExcell;
        #endregion

        #region KirimPage
        HintAssist.SetHint(_inputPage.txtProductSearch, DeLong_Desktop.Resources.Resource.Mahsulotni_qidirish_);
        _inputPage.dtmahsulot.Header = DeLong_Desktop.Resources.Resource.Product;
        #endregion

        #region SalePracticePage
        _salePracticePage.lbSotuvPage.Content = DeLong_Desktop.Resources.Resource.Sotuvni_amalga_oshirish_;
        _salePracticePage.btnMahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.tbdollorkursi.Text = DeLong_Desktop.Resources.Resource.Dollar_kursi_;
        _salePracticePage.btnDollarKurs.Content = DeLong_Desktop.Resources.Resource.Dollar_kursini_kiriting_;
        _salePracticePage.btnMiqdori.Content = DeLong_Desktop.Resources.Resource.Miqdori_;
        HintAssist.SetHint(_salePracticePage.cbxProduct, DeLong_Desktop.Resources.Resource.Mahsulotni_tanlang_);
        _salePracticePage.btnProductSell.Content = DeLong_Desktop.Resources.Resource.Add;
        _salePracticePage.lblmahsulot.Content = DeLong_Desktop.Resources.Resource.Product;
        _salePracticePage.ProductGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.T_r_;
        _salePracticePage.ProductGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Mahsulot_nomi_;
        _salePracticePage.ProductGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Narxi_;
        _salePracticePage.ProductGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Miqdori_;
        _salePracticePage.ProductGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Olchov_birligi;
        _salePracticePage.ProductGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Umumiy_summasi_;
        _salePracticePage.ProductGrid.Columns[6].Header = DeLong_Desktop.Resources.Resource.Ochirish;
        HintAssist.SetHint(_salePracticePage.cbxPayment, DeLong_Desktop.Resources.Resource.Mijozni_tanlang_);
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
        _productpage.dataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.ProductSign;
        _productpage.dataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Ombordagi_miqdor_;
        _productpage.dataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Kategoriya;
        _productpage.dataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Faolligi_;
        _productpage.dataGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Amallar_;
        #endregion

        #region MainWindow
        btnCash.Content = DeLong_Desktop.Resources.Resource.Cash;
        btnChiqish.Content = DeLong_Desktop.Resources.Resource.Exit;
        btnHisobot.Content = DeLong_Desktop.Resources.Resource.Report;
        btnAdditionalOperations.Content = DeLong_Desktop.Resources.Resource.AdditionalOperations;

        #region MainWindow Navigation Items
        if (SidenavMenuSales.Items.Count > 0 && SidenavMenuSales.Items[0] is TreeViewItem salesItem)
        {
            if (salesItem.Header is StackPanel salesHeaderPanel && salesHeaderPanel.Children.Count > 1)
            {
                if (salesHeaderPanel.Children[1] is TextBlock salesText)
                {
                    salesText.Text = DeLong_Desktop.Resources.Resource.Sales;
                }
            }

            if (salesItem.Items.Count > 0 && salesItem.Items[0] is TreeViewItem salesOperationsItem)
            {
                if (salesOperationsItem.Header is StackPanel operationsPanel && operationsPanel.Children.Count > 1)
                {
                    if (operationsPanel.Children[1] is TextBlock operationsText)
                    {
                        operationsText.Text = DeLong_Desktop.Resources.Resource.SalesOperations;
                    }
                }
            }

            if (salesItem.Items.Count > 1 && salesItem.Items[1] is TreeViewItem salesHistoryItem)
            {
                if (salesHistoryItem.Header is StackPanel historyPanel && historyPanel.Children.Count > 1)
                {
                    if (historyPanel.Children[1] is TextBlock historyText)
                    {
                        historyText.Text = DeLong_Desktop.Resources.Resource.SalesHistory;
                    }
                }
            }
        }

        if (SidenavMenuProducts.Items.Count > 0 && SidenavMenuProducts.Items[0] is TreeViewItem productsItem)
        {
            if (productsItem.Header is StackPanel productsHeaderPanel && productsHeaderPanel.Children.Count > 1)
            {
                if (productsHeaderPanel.Children[1] is TextBlock productsText)
                {
                    productsText.Text = DeLong_Desktop.Resources.Resource.Product;
                }
            }

            if (productsItem.Items.Count > 0 && productsItem.Items[0] is TreeViewItem productsList)
            {
                if (productsList.Header is StackPanel productsListPanel && productsListPanel.Children.Count > 1)
                {
                    if (productsListPanel.Children[1] is TextBlock productsListText)
                    {
                        productsListText.Text = DeLong_Desktop.Resources.Resource.Products;
                    }
                }
            }

            if (productsItem.Items.Count > 1 && productsItem.Items[1] is TreeViewItem incomingItem)
            {
                if (incomingItem.Header is StackPanel incomingPanel && incomingPanel.Children.Count > 1)
                {
                    if (incomingPanel.Children[1] is TextBlock incomingText)
                    {
                        incomingText.Text = DeLong_Desktop.Resources.Resource.Incoming;
                    }
                }
            }
            if (productsItem.Items.Count > 2 && productsItem.Items[2] is TreeViewItem historyItem)
            {
                if (historyItem.Header is StackPanel historyPanel && historyPanel.Children.Count > 2)
                {
                    if (historyPanel.Children[1] is TextBlock historyText)
                    {
                        historyText.Text = DeLong_Desktop.Resources.Resource.HistoryInput;
                    }
                }
            }
        }

        // Sozlash bo'limi uchun kod
        if (settingsTreeItem != null)
        {
            // Sozlash header qismi
            if (settingsTreeItem.Header is StackPanel settingsHeaderPanel && settingsHeaderPanel.Children.Count > 1)
            {
                if (settingsHeaderPanel.Children[1] is TextBlock settingsText)
                {
                    settingsText.Text = DeLong_Desktop.Resources.Resource.Settings;
                }
            }

            // Tillar qismi
            if (settingsTreeItem.Items.Count > 0 && settingsTreeItem.Items[0] is TreeViewItem languagesItem)
            {
                if (languagesItem.Header is StackPanel languagesPanel && languagesPanel.Children.Count > 1)
                {
                    if (languagesPanel.Children[1] is TextBlock languagesText)
                    {
                        languagesText.Text = DeLong_Desktop.Resources.Resource.Languages;
                    }
                }
            }

            // Filiallar qismi
            if (settingsTreeItem.Items.Count > 1 && settingsTreeItem.Items[1] is TreeViewItem branchesItem)
            {
                if (branchesItem.Header is StackPanel branchesPanel && branchesPanel.Children.Count > 1)
                {
                    if (branchesPanel.Children[1] is TextBlock branchesText)
                    {
                        branchesText.Text = DeLong_Desktop.Resources.Resource.Branches;
                    }
                }
            }
        }

        // Foydalanuvchilar bo'limi uchun kod
        if (SidenavMenuUsers.Items.Count > 0 && SidenavMenuUsers.Items[0] is TreeViewItem usersItem)
        {
            // Foydalanuvchi header qismi
            if (usersItem.Header is StackPanel usersHeaderPanel && usersHeaderPanel.Children.Count > 1)
            {
                if (usersHeaderPanel.Children[1] is TextBlock usersText)
                {
                    usersText.Text = DeLong_Desktop.Resources.Resource.Users;
                }
            }

            // Taminotchilar qismi
            if (usersItem.Items.Count > 0 && usersItem.Items[0] is TreeViewItem suppliersItem)
            {
                if (suppliersItem.Header is StackPanel suppliersPanel && suppliersPanel.Children.Count > 1)
                {
                    if (suppliersPanel.Children[1] is TextBlock suppliersText)
                    {
                        suppliersText.Text = DeLong_Desktop.Resources.Resource.Suppliers;
                    }
                }
            }

            // Mijozlar qismi
            if (usersItem.Items.Count > 1 && usersItem.Items[1] is TreeViewItem customersItem)
            {
                if (customersItem.Header is StackPanel customersPanel && customersPanel.Children.Count > 1)
                {
                    if (customersPanel.Children[1] is TextBlock customersText)
                    {
                        customersText.Text = DeLong_Desktop.Resources.Resource.Customers;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region AdditionalOperationPage
        HintAssist.SetHint(_additionalOperationsPage.tbSearchDebt, DeLong_Desktop.Resources.Resource.Search);
        HintAssist.SetHint(_additionalOperationsPage.tbSaleId, DeLong_Desktop.Resources.Resource.Chek_Id);
        HintAssist.SetHint(_additionalOperationsPage.tbReturnedFrom, DeLong_Desktop.Resources.Resource.Kimdan_qaytmoqda);
        HintAssist.SetHint(_additionalOperationsPage.cbSalePriceProducts, DeLong_Desktop.Resources.Resource.Mahsulotni_tanlang_);
        HintAssist.SetHint(_additionalOperationsPage.tbDollarPayment, DeLong_Desktop.Resources.Resource.Dollar);
        HintAssist.SetHint(_additionalOperationsPage.tbCardPayment, DeLong_Desktop.Resources.Resource.Plastik_);
        HintAssist.SetHint(_additionalOperationsPage.tbCashPayment, DeLong_Desktop.Resources.Resource.Naqd_);
        HintAssist.SetHint(_additionalOperationsPage.tbReturnQuantity, DeLong_Desktop.Resources.Resource.Miqdori_);
        HintAssist.SetHint(_additionalOperationsPage.tbUnitOfMeasure, DeLong_Desktop.Resources.Resource.Olchov_birligi);
        HintAssist.SetHint(_additionalOperationsPage.tbReturnAmount, DeLong_Desktop.Resources.Resource.Umumiy_summasi_);
        HintAssist.SetHint(_additionalOperationsPage.tbComment, DeLong_Desktop.Resources.Resource.Izoh);
        HintAssist.SetHint(_additionalOperationsPage.cbProductList, DeLong_Desktop.Resources.Resource.Mahsulot_nomi_);
        HintAssist.SetHint(_additionalOperationsPage.tbQuantity, DeLong_Desktop.Resources.Resource.Miqdori_);
        HintAssist.SetHint(_additionalOperationsPage.cbToWarehouse, DeLong_Desktop.Resources.Resource.Qaysi_omborga);
        HintAssist.SetHint(_additionalOperationsPage.cbTransactionType, DeLong_Desktop.Resources.Resource.TransactionType);
        HintAssist.SetHint(_additionalOperationsPage.tbCommentProvodka, DeLong_Desktop.Resources.Resource.Izoh);
        _additionalOperationsPage.additionalActions.Text = DeLong_Desktop.Resources.Resource.Qoshimcha_amallar;
        _additionalOperationsPage.tbqarznitolash.Text = DeLong_Desktop.Resources.Resource.Qarzni_tolash;
        _additionalOperationsPage.dtmijoznomi.Header = DeLong_Desktop.Resources.Resource.Mijoz_nomi;
        _additionalOperationsPage.dtqarzsummasi.Header = DeLong_Desktop.Resources.Resource.Qarz_summasi;
        _additionalOperationsPage.dttolashmuddati.Header = DeLong_Desktop.Resources.Resource.Tolash_muddati;
        _additionalOperationsPage.tbTotalDebtLabel.Text = DeLong_Desktop.Resources.Resource.Jami_qarz;
        _additionalOperationsPage.btnPayAllDebts.Content = DeLong_Desktop.Resources.Resource.Tolash;
        _additionalOperationsPage.tbqaytganmahsulotlar.Text = DeLong_Desktop.Resources.Resource.Qaytgan_mahsulotlar;
        _additionalOperationsPage.btnAddProduct.Content = DeLong_Desktop.Resources.Resource.Add;
        _additionalOperationsPage.btnConfirmReturn.Content = DeLong_Desktop.Resources.Resource.Confirm;
        _additionalOperationsPage.tbomborlarortasidaotkazmalar.Text = DeLong_Desktop.Resources.Resource.Omborlar_ortasida_otkazmalar;
        _additionalOperationsPage.transferDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Mahsulot_nomi_;
        _additionalOperationsPage.transferDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.Miqdori_;
        _additionalOperationsPage.transferDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Olchov_birligi;
        _additionalOperationsPage.transferDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Mahsulot_tannarxi;
        _additionalOperationsPage.transferDataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Umumiy_summasi_;
        _additionalOperationsPage.transferDataGrid.Columns[5].Header = DeLong_Desktop.Resources.Resource.Ochirish;
        _additionalOperationsPage.btnSaveTransfer.Content = DeLong_Desktop.Resources.Resource.Confirm;
        #endregion

        #region SaleHistoryPage
        _saleHistoryPage.tbsotuvlartarixi.Text = DeLong_Desktop.Resources.Resource.Sotuvlar_tarixi;
        _saleHistoryPage.saleDataGrid.Columns[0].Header = DeLong_Desktop.Resources.Resource.Chek_Id;
        _saleHistoryPage.saleDataGrid.Columns[1].Header = DeLong_Desktop.Resources.Resource.CustomerName;
        _saleHistoryPage.saleDataGrid.Columns[2].Header = DeLong_Desktop.Resources.Resource.Umumiy_summasi_;
        _saleHistoryPage.saleDataGrid.Columns[3].Header = DeLong_Desktop.Resources.Resource.Sana;
        _saleHistoryPage.saleDataGrid.Columns[4].Header = DeLong_Desktop.Resources.Resource.Pechat;
        HintAssist.SetHint(_saleHistoryPage.txtSearch, DeLong_Desktop.Resources.Resource.Search);
        #endregion
    }

    private void btnChiqish_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnMaxsulot_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(_productpage);
        UpdateLanguage();
    }

    private void OnAdditionalOperationsClick(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(_additionalOperationsPage);
        UpdateLanguage();
    }

    private void btnCash_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(_cashPage);
        UpdateLanguage();
    }

    private void btnReport_Click(object sender, RoutedEventArgs e)
    {
        var reportPage = new ReportPage(_services);
        Navigator.Navigate(reportPage);
    }


    private void tbSaleHistory_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Navigator.Navigate(_saleHistoryPage);
        UpdateLanguage();
    }

    private void tbSaleOperation_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Navigator.Navigate(_salePracticePage);
        UpdateLanguage();
    }

    private void tbProducts_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Navigator.Navigate(_productpage);
        UpdateLanguage();
    }

    private void tbIncoming_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Navigator.Navigate(_inputPage);
        UpdateLanguage();
    }
}