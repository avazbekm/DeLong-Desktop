using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.SalesPractice;

/// <summary>
/// Interaction logic for SalePracticePage.xaml
/// </summary>
public partial class SalePracticePage : Page
{
    private readonly IPriceService priceService;
    private readonly IProductService productService;
    private readonly IServiceProvider _services;
    public SalePracticePage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        productService = _services.GetRequiredService<IProductService>();
        priceService = _services.GetRequiredService<IPriceService>();

        LoadingProductData();
    }

    private async void LoadingProductData()
    {
        try
        {
            // Ma'lumotlarni olish
            var products = await productService.RetrieveAllAsync();
            if (products == null || !products.Any())
            {
                MessageBox.Show("Mahsulotlar mavjud emas!");
                return;
            }

            cbxProduct.Items.Clear();

            // ComboBox kolleksiyasi uchun yangi ro'yxat yaratish
            var comboboxItems = new List<ComboboxItem>();
       
            // Mahsulotlarni qo'shish
            comboboxItems.AddRange(products.Select(product => new ComboboxItem
            {
                Id = product.Id,
                ProductName = char.ToUpper(product.Name[0]) + product.Name.Substring(1),
            }));

            // ComboBox'ga yangi ro'yxatni bog'lash
            cbxProduct.ItemsSource = comboboxItems;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}");
        }
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Matn maydonini olish
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat musbat raqamlar va bir dona nuqta (.) qoldirish
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == '.').ToArray());

            // Nuqta faqat bir marta bo'lishiga ruxsat
            int dotCount = filteredText.Count(c => c == '.');
            if (dotCount > 1)
            {
                int lastDotIndex = filteredText.LastIndexOf('.');
                filteredText = filteredText.Remove(lastDotIndex, 1); // Oxirgi nuqtani olib tashlash
            }

            // Agar matn o'zgargan bo'lsa, uni qayta o'rnating
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Joriy kursor pozitsiyasini saqlash
                textBox.Text = filteredText; // Tozalangan matnni o'rnatish
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Kursorni to'g'ri joyda saqlash
            }
        }
    }

    private void cbxProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedProduct = cbxProduct.SelectedItem as ComboboxItem;
        if (selectedProduct != null)
        {
            SalePracticeInfo.ProductId = selectedProduct.Id;
            SalePracticeInfo.ProductName = selectedProduct.ProductName;
        }
    }

    List<ProductItem> items = new List<ProductItem>();
    private async void btnProductSell_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // belgilangan mahsulot narxlarini chaqirib olamiz
            var prices = await priceService.RetrieveAllAsync(SalePracticeInfo.ProductId);

            // sonini decimalga o'tkazib olamiz
            decimal quantity = decimal.Parse(tbQuantity.Text);

            int quantityIndex = 0;

            foreach (var price in prices) 
            {
                quantityIndex++;
                if (price.Quantity >= quantity)
                {
                    items.Add(new ProductItem()
                    {
                        PriceId = price.Id,
                        ProductName = SalePracticeInfo.ProductName,
                        SerialNumber = quantityIndex,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = quantity
                    });
                    ProductGrid.ItemsSource = items;

                    PriceUpdateDto priceUpdateDto = new PriceUpdateDto()
                    {
                        Id = price.Id,
                        ArrivalPrice = price.ArrivalPrice,
                        ProductId = price.ProductId,
                        Quantity = price.Quantity - quantity,
                        SellingPrice = price.SellingPrice,
                        UnitOfMeasure = price.UnitOfMeasure
                    };

                    bool result = await priceService.ModifyAsync(priceUpdateDto);
                    if (!result)
                    {
                        MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                }
                else if (price.Quantity < quantity && prices.Count().Equals(1))
                {
                    items.Add(new ProductItem()
                    {
                        ProductName = SalePracticeInfo.ProductName,
                        PriceId = price.Id,
                        SerialNumber = quantityIndex,
                        Price = price.SellingPrice,
                        Unit = price.UnitOfMeasure,
                        Quantity = price.Quantity
                    });
                    ProductGrid.ItemsSource = items;
                    PriceUpdateDto priceUpdateDto = new PriceUpdateDto()
                    {
                        Id = price.Id,
                        ArrivalPrice = price.ArrivalPrice,
                        ProductId = price.ProductId,
                        Quantity = 0,
                        SellingPrice = price.SellingPrice,
                        UnitOfMeasure = price.UnitOfMeasure
                    };

                    bool result = await priceService.ModifyAsync(priceUpdateDto);
                    if (!result)
                    {
                        MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else 
                {
                    
                }
            }
        }
        catch 
        {
            
        }
    }
}