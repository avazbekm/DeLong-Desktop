using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Windows.Products;
using Page = System.Windows.Controls.Page;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Products;

/// <summary>
/// Interaction logic for ProductsPage.xaml
/// </summary>
public partial class ProductsPage : Page
{
    private readonly IProductService productService;
    private readonly ICategoryService categoryService;
    private readonly IPriceService priceService;
    private readonly IServiceProvider services;

    public ProductsPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();

        LoadCategoriesAsync();
        LoadData();
    }

    // Kategoriyalarni yuklash
    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await this.categoryService.RetrieveAllAsync();

            // Kategoriya elementlarini ComboBoxga qo'shish
            var comboBoxItems = new List<ItemCombobox>();
            foreach (var category in categories)
            {
                comboBoxItems.Add(new ItemCombobox
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            // ComboBoxni ma'lumotlar bilan to'ldirish
            cmbCategory.ItemsSource = comboBoxItems;
            cmbCategory.DisplayMemberPath = "Name"; // Ko'rsatiladigan nom
            cmbCategory.SelectedValuePath = "Id";  // Tanlangan qiymat

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}");
        }
    }

    private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbCategory.SelectedValue is long selectedCategoryId)
        {
            // dataGrid.ItemSource ni tozalaymiz.
            dataGrid.ItemsSource = null;
            
            // Bu yerda tanlangan kategoriya bo'yicha mahsulotlarni filtrlashni qo'shishingiz mumkin
            LoadData(selectedCategoryId);
        }
    }

    private async void LoadData(long categoryId)
    {
        // dataGrid.ItemSource ni tozalaymiz.
        dataGrid.ItemsSource = null;

        // Mahsulotlar ro'yxati
        List<Item> items = new List<Item>();

        // Narxlarni olish
        var prices = await priceService.RetrieveAllAsync();

        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null && prices is not null)
        {
            foreach (var product in products)
            {
                // Tanlangan kategoriya bo'yicha mahsulotlarni filtrlaymiz
                if (product.CategoryId.Equals(categoryId))
                {
                    // Shu mahsulotga tegishli barcha narxlarni topamiz
                    var productPrices = prices.Where(p => p.ProductId.Equals(product.Id));

                    foreach (var price in productPrices)
                    {
                        items.Add(new Item()
                        {
                            Id = product.Id,
                            Name = product.Name.ToUpper(),
                            Description = product.Description.ToUpper(),
                            Quantity = price.Quantity,
                            UnitOfMesure = price.UnitOfMeasure,
                            Price = price.SellingPrice,
                            IsActive = product.IsActive
                        });
                    }
                }
            }
        }

        // DataGrid'ni ma'lumot bilan to'ldirish
        dataGrid.ItemsSource = items;
    }

    private async void LoadData()
    {
        // dataGrid.ItemSource ni tozalaymiz.
        dataGrid.ItemsSource = null;

        // Mahsulotlar ro'yxati
        List<Item> items = new List<Item>();

        // Narxlarni olish
        var prices = await priceService.RetrieveAllAsync();

        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null && prices is not null)
        {
            foreach (var product in products)
            {
                // Shu mahsulotga tegishli barcha narxlarni topamiz
                var productPrices = prices.Where(p => p.ProductId.Equals(product.Id));

                foreach (var price in productPrices)
                {
                    items.Add(new Item()
                    {
                        Id = product.Id,
                        Name = product.Name.ToUpper(),
                        Description = product.Description.ToUpper(),
                        Quantity = price.Quantity,
                        UnitOfMesure = price.UnitOfMeasure,
                        Price = price.SellingPrice,
                        IsActive = product.IsActive
                    });
                }
            }
        }

        // DataGrid'ni ma'lumot bilan to'ldirish
        dataGrid.ItemsSource = items;
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        ProductAddWindow productAddWindow = new ProductAddWindow(services);
        productAddWindow.ShowDialog();
    }
}
