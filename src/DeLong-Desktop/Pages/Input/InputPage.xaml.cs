using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.Pages.Products;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using static MaterialDesignThemes.Wpf.Theme;

namespace DeLong_Desktop.Pages.Input;

/// <summary>
/// Interaction logic for InputPage.xaml
/// </summary>
public partial class InputPage : Page
{
    private readonly IServiceProvider services;
    private readonly IProductService productService;
    private readonly ICategoryService categoryService;
    private readonly IPriceService priceService;

    public InputPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();

        LoadCategoriesAsync();
        LoadProductsAsync();

    }
    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await this.categoryService.RetrieveAllAsync();

            // Kategoriya elementlarini ComboBoxga qo'shish
            var datagridItems = new List<ItemCategory>();
            foreach (var category in categories)
            {
                datagridItems.Add(new ItemCategory
                {
                    Id = category.Id,
                    Category = category.Name
                });
            }
            categoryDataGrid.ItemsSource = datagridItems;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}");
        }
    }

    private async void LoadProductsAsync()
    {
        // dataGrid.ItemSource ni tozalaymiz.
        productDataGrid.ItemsSource = string.Empty;

        // Mahsulotlar ro'yxati
        List<ItemProduct> items = new List<ItemProduct>();


        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null)
        {
            foreach (var product in products)
            {
                items.Add(new ItemProduct()
                {
                    Id = product.Id,
                    Product = product.Name.ToUpper(),
                });
            }
        }

        // DataGrid'ni ma'lumot bilan to'ldirish
        productDataGrid.ItemsSource = items;
    }
    private void rbtnCategory_Checked(object sender, RoutedEventArgs e)
    {
            // Headerni 🔳 ga o'zgartirish
            radioColumn.Header = "🔳";

    }
}
