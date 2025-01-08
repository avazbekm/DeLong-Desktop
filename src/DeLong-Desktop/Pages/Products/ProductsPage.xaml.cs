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
            dataGrid.ItemsSource = string.Empty;
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
                    items.Add(new Item()
                    {
                        Id = product.Id,
                        Name = product.Name.ToUpper(),
                        Description = product.Description.ToUpper(),
                        Category = category.Name.ToUpper(),
                        IsActive = product.IsActive,
                        CategoryId = category.Id
                    });
                }
            }
        }

        // DataGrid'ni ma'lumot bilan to'ldirish
        dataGrid.ItemsSource = items;
    }

    private async void LoadData()
    {
        // dataGrid.ItemSource ni tozalaymiz.
        dataGrid.ItemsSource = string.Empty;

        // Mahsulotlar ro'yxati
        List<Item> items = new List<Item>();

        // kategoriyalar olish
        var categories  = await categoryService.RetrieveAllAsync();

        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null)
        {
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));
                items.Add(new Item()
                {
                    Id = product.Id,
                    Name = product.Name.ToUpper(),
                    Description = product.Description.ToUpper(),
                    Category = category.Name.ToUpper(),
                    IsActive = product.IsActive,
                    CategoryId = category.Id
                });
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

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtSearch.Text.Trim();
        FilterProducts(searchText);
    }

    private async void FilterProducts(string searchText)
    {
        long categoryId = 0;
        dataGrid.ItemsSource = string.Empty;

        if (cmbCategory.SelectedValue is long selectedCategoryId)
            categoryId = selectedCategoryId;
        
        // Mahsulotlar ro'yxati
        List<Item> items = new List<Item>();

        // kategoriyalar olish
        var categories = await categoryService.RetrieveAllAsync();

        // Mahsulotlarni olish
        var products = await productService.RetrieveAllAsync();

        // Agar mahsulotlar mavjud bo'lsa, ularni ro'yxatga qo'shish
        if (products is not null)
        {
            if (categoryId == 0)
            {
                foreach (var product in products)
                {
                    var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));

                        if (product.Name.Contains(searchText.ToLower()))
                            items.Add(new Item()
                            {
                                Id = product.Id,
                                Name = product.Name.ToUpper(),
                                Description = product.Description.ToUpper(),
                                Category = category.Name.ToUpper(),
                                IsActive = product.IsActive,
                                CategoryId = category.Id
                            });
                }
            }
            else 
            {
                foreach (var product in products)
                {
                    if (product.Name.Contains(searchText.ToLower()) && product.CategoryId == categoryId)
                    {
                        var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));
                        items.Add(new Item()
                        {
                            Id = product.Id,
                            Name = product.Name.ToUpper(),
                            Description = product.Description.ToUpper(),
                            Category = category.Name.ToUpper(),
                            IsActive = product.IsActive,
                            CategoryId = category.Id
                        });
                    }
                }
            }
            dataGrid.ItemsSource = items;
        }
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        if (dataGrid.SelectedItem is Item selectedProduct)
        {
            ProductInfo.ProductId = selectedProduct.Id;
            ProductInfo.CategoryId = selectedProduct.CategoryId;
        }
        ProductEditWindow productEditWindow = new ProductEditWindow(services);
        productEditWindow.spCategory.Visibility = Visibility.Visible;

        productEditWindow.ShowDialog();
    }
}
