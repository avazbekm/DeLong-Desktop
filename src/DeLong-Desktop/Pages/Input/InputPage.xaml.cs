using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
    private async void LoadCategoriesAsync()
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
                    Category = category.Name.ToUpper()
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
        if (categoryDataGrid.SelectedItem is ItemCategory selectedCategory)
        {
            InputInfo.CategoryId = selectedCategory.Id;
            LoadDataAsync(selectedCategory.Id);
        }
    }
    private async void LoadDataAsync(long categoryId)
    {
        // dataGrid.ItemSource ni tozalaymiz.
        productDataGrid.ItemsSource = null;

        // Mahsulotlar ro'yxati
        List<ItemProduct> items = new List<ItemProduct>();

        // Narxlarni olish
        var categories = await categoryService.RetrieveAllAsync();

        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null)
        {
            foreach (var product in products)
            {
                if (product.CategoryId == categoryId)
                {
                    var category = categories.FirstOrDefault(p => p.Id.Equals(categoryId));
                    items.Add(new ItemProduct()
                    {
                        Id = product.Id,
                        Product = product.Name.ToUpper()
                    });
                }
            }
        }

        // DataGrid'ni ma'lumot bilan to'ldirish
        productDataGrid.ItemsSource = items;
    }
    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtSearch.Text.Trim();
        FilterCategoriya(searchText);
    }
    private async void FilterCategoriya(string searchText) 
    {
        categoryDataGrid.ItemsSource = string.Empty;
        
        // datagrid to'ldirish uchun
        List<ItemCategory> items = new List<ItemCategory>();

        // kategoriyalar olish
        var categories = await categoryService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (categories is not null)
        {
            foreach (var category in categories)
            {

                if (category.Name.Contains(searchText.ToLower()))
                    items.Add(new ItemCategory()
                    {
                        Id = category.Id,
                        Category = category.Name.ToUpper(),
                    });
            }
            categoryDataGrid.ItemsSource = items;
        }
    }
    private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtProductSearch.Text.Trim();

        if (InputInfo.CategoryId == 0)
        {
            FilterProductsAsync(searchText);
        }
        else
        {
            FilterProductsAsync(searchText, InputInfo.CategoryId);
        }
    }
    private async void FilterProductsAsync(string searchText)
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
                if(product.Name.Contains(searchText.ToLower()))
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
    private async void FilterProductsAsync(string searchText, long categoryId)
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
                if (product.Name.Contains(searchText.ToLower()) && product.CategoryId == categoryId)
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
    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        rbtnProductHeader.Header = "🔳";
        if (productDataGrid.SelectedItem is ItemProduct product)
        { 
            tbProductName.Text = product.Product.ToUpper();
        }
    }
}
