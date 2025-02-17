using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DeLong_Desktop.Windows.DollarKurs;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace DeLong_Desktop.Pages.SalesPractice;

public partial class SalePracticePage : Page
{
    private readonly IPriceService _priceService;
    private readonly IProductService _productService;
    private readonly IKursDollarService kursDollarService;
    private readonly IServiceProvider services;

    private TextBox lastUpdatedTextBox;

    public ObservableCollection<ProductItem> Items { get; set; } = new();

    public SalePracticePage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _productService = services.GetRequiredService<IProductService>();
        _priceService = services.GetRequiredService<IPriceService>();
        kursDollarService = services.GetRequiredService<IKursDollarService>();

        ProductGrid.ItemsSource = Items; // Faqat 1 marta bog'lash

        // ObservableCollection o'zgarishlarini kuzatish
        Items.CollectionChanged += (s, e) => UpdateTotalSum();

        LoadingProductData();

        LoadDollarRate(); // Dollar kursini yuklash
    }
    //kursni databazadan olib kelish
    private async void LoadDollarRate()
    {
        try
        {
            var latestRate = await kursDollarService.RetrieveByIdAsync();
            if (latestRate != null && latestRate.TodayDate.Equals(DateTime.Now.ToString("dd.MM.yyyy")))
            {
                tbDolarKurs.Text = latestRate.SellingDollar.ToString("F2"); // Formatlangan kurs qiymatini ko'rsatish
            }
            else
                MessageBox.Show("Bugungi dollar kursni o'rnating.","Kurs ma'lumot");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Dollar kursini yuklashda xatolik: {ex.Message}");
        }
    }
    private void UpdateTotalSum()
    {
        decimal totalSum = Items.Sum(item => item.TotalPrice);
        tbTotalPrice.Text = totalSum.ToString("N2");
    }
    // comboboxni product bilan to'ldiramiz
    private async void LoadingProductData()
    {
        try
        {
            var products = await _productService.RetrieveAllAsync();
            if (products == null || !products.Any())
            {
                return;
            }

            var comboboxItems = products.Select(product => new ComboboxItem
            {
                Id = product.Id,
                ProductName = char.ToUpper(product.Name[0]) + product.Name[1..]
            }).ToList();

            cbxProduct.ItemsSource = comboboxItems;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }
    }

    private async void btnProductSell_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cbxProduct.SelectedItem is not ComboboxItem selectedProduct ||
                !decimal.TryParse(tbQuantity.Text, out decimal quantity))
            {
                MessageBox.Show("Mahsulotni tanlang yoki miqdorini kiriting!");
                return;
            }

            var prices = await _priceService.RetrieveAllAsync(selectedProduct.Id);
            if (prices == null) return;

            foreach (var price in prices)
            {
                if (price.Quantity >= quantity)
                {
                    var newItem = new ProductItem
                    {
                        PriceId = price.Id,
                        ProductName = selectedProduct.ProductName,
                        SerialNumber = Items.Count + 1,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = quantity,
                        CostPrice = price.CostPrice,
                        ProductId = price.ProductId,
                        BalanceAmount = price.Quantity
                    };

                    Items.Add(newItem); // Avtomatik UI yangilanadi
                    break;

                    //var priceUpdateDto = new PriceUpdateDto
                    //{
                    //    Id = price.Id,
                    //    Quantity = price.Quantity - quantity,
                    //    ArrivalPrice = price.ArrivalPrice,
                    //    SellingPrice = price.SellingPrice,
                    //    UnitOfMeasure = price.UnitOfMeasure,
                    //    ProductId = price.ProductId
                    //};

                    //await _priceService.ModifyAsync(priceUpdateDto);
                }
                else if(price.Quantity < quantity)
                {
                    var newItem = new ProductItem
                    {
                        PriceId = price.Id,
                        ProductName = selectedProduct.ProductName,
                        SerialNumber = Items.Count + 1,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = price.Quantity,
                        CostPrice = price.CostPrice,
                        ProductId = price.ProductId,
                        BalanceAmount = price.Quantity
                    };

                    Items.Add(newItem); // Avtomatik UI yangilanadi
                    quantity -= price.Quantity;

                    if (quantity == 0)
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }

    }

    private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button.DataContext as ProductItem; //  DataGrid elementlari tipi

        if (item != null && ProductGrid.ItemsSource is ObservableCollection<ProductItem> items)
        {
            items.Remove(item);
        }
    }

    private void UpdateTotalSumm()
    {
        try
        {
            // Parse values from tbCrashSum and tbqarz
            double crashSum = ParseDouble(tbCrashSum.Text);
            double qarzSum = ParseDouble(tbqarz.Text);
            double plastikSum = ParseDouble(tbPlastikSum.Text);
            double dollar = ParseDouble(tbDollar.Text);
            double kurs = ParseDouble(tbDolarKurs.Text);
            double chegirma = ParseDouble(tbDiscount.Text);

            // Calculate the total and display it in tbQoldiq
            tbQoldiq.Text = (crashSum + qarzSum + plastikSum + dollar * kurs).ToString("N2");
            double qoldi = ParseDouble(tbQoldiq.Text);

            // Umumiy summa jamlaymiz
            double gridTotalSum = Items.Sum(item => (double)item.TotalPrice);
            if (gridTotalSum - chegirma < 0 ||
                gridTotalSum - qoldi < 0 ||
                gridTotalSum - qoldi - chegirma < 0)
            {
                string message = "Umumiy summa minus bo'lishi mumkin emas.";
                if (lastUpdatedTextBox != null)
                {
                    lastUpdatedTextBox.Text = null;
                }
                MessageBox.Show(message);
                return;
            }
            tbTotalPrice.Text = (gridTotalSum - qoldi - chegirma).ToString("N2");
        }
        catch
        {
            MessageBox.Show("Bugungi dollar kursini o'rnating.");
        }
    }

    private double ParseDouble(string input)
    {
        // Helper method to safely parse numbers
        double.TryParse(input, out double value);
        return value;
    }

    private void tbCrashSum_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbPlastikSum_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbDollar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (tbDolarKurs.Text == "")
        {
            tbDollar.Text = null;
            MessageBox.Show("Bugungi dollar kursini o'rnating.");
            return;
        }
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbqarz_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void tbDiscount_TextChanged(object sender, TextChangedEventArgs e)
    {
        TrackTextBoxUpdate(sender);
        ValidateAndCleanInput(sender);
    }

    private void TrackTextBoxUpdate(object sender)
    {
        if (sender is TextBox textBox)
        {
            lastUpdatedTextBox = textBox; // oxirgi yangilangan textbox ni saqlaymiz
        }
    }

    private void ValidateAndCleanInput(object sender)
    {
        if (sender is TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;

            // Kiritilgan matn
            string input = textBox.Text;

            // Tozalangan matn (faqat raqam va bitta nuqta)
            string cleanInput = "";
            bool hasDot = false;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    cleanInput += c;
                }
                else if (c == '.' && !hasDot)
                {
                    cleanInput += c;
                    hasDot = true;
                }
            }

            // Agar matn o'zgarsa, yangilaymiz
            if (textBox.Text != cleanInput)
            {
                textBox.Text = cleanInput;
                textBox.CaretIndex = Math.Min(caretIndex, cleanInput.Length);
            }
        }
        UpdateTotalSumm();
    }

    private void btnDollarKurs_Click(object sender, RoutedEventArgs e)
    {
        DollarKursWindow dollarKursWindow = new DollarKursWindow(services);
        dollarKursWindow.ShowDialog();
        LoadDollarRate(); // Dollar kursini yuklash
    }

    private void tbQuatity_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAndCleanInput(sender);
        if (ProductGrid.SelectedItem is ProductItem selectedItem)
        {

            if (sender is TextBox textBox)
            {
                var dataGridRow = FindParent<DataGridRow>(textBox);
                if (dataGridRow != null)
                {
                    var productItem = dataGridRow.Item as ProductItem;
                    if (productItem != null)
                    {
                        decimal.TryParse(textBox.Text, out decimal newQuantity);
                        if (selectedItem.BalanceAmount < newQuantity)
                        {
                            MessageBox.Show($"{selectedItem.ProductName} " +
                                $"mahsulotning bazadagi qoldiq miqdori: {selectedItem.BalanceAmount} {selectedItem.Unit}");
                            productItem.Quantity = selectedItem.BalanceAmount;
                        }
                        else
                            productItem.Quantity = newQuantity;

                        UpdateTotalSum();
                    }
                }
            }
        }
    }
    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        if (parent == null) return null;

        if (parent is T parentType) return parentType;

        return FindParent<T>(parent);
    }

    private void btnSellDollar_Click(object sender, RoutedEventArgs e)
    {
        DollarSellWindow dollarSellWindow = new DollarSellWindow(services);
        dollarSellWindow.ShowDialog();
    }

    private void btnBuyDollar_Click(object sender, RoutedEventArgs e)
    {
        DollarBuyWindow dollarBuyWindow = new DollarBuyWindow(services);
        dollarBuyWindow.ShowDialog();
    }
}