using System.Windows;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Category;
using DeLong_Desktop.ApiService.DTOs.Products;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Products;

/// <summary>
/// Interaction logic for ProductAddWindow.xaml
/// </summary>
public partial class ProductAddWindow : Window
{
    private readonly ICategoryService categoryService;
    private readonly IProductService productService;
    private readonly IPriceService priceService;
    private readonly IServiceProvider services;

    public ProductAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();
    }

    private async void rbtnProduct_Checked(object sender, RoutedEventArgs e)
    {
        spProduct.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        grProductShow.Visibility = Visibility.Visible;
        grCategoryShow.Visibility = Visibility.Collapsed;

        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

        if (products is not null)
        {
            int Number = 0;
            productDataGrid.ItemsSource = string.Empty;
            foreach (var product in products)
            {
                items.Add(new ItemProduct()
                {
                    Id = product.Id,
                    TartibRaqam = ++Number,
                    Name = product.Name.ToUpper()
                });
            }
            productDataGrid.ItemsSource = items;
        }
    }

    private async void rbtnCategory_Checked(object sender, RoutedEventArgs e)
    {
        spProduct.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Visible;
        spJisNew.Visibility = Visibility.Collapsed;
        grCategoryShow.Visibility = Visibility.Visible;
        grProductShow.Visibility = Visibility.Collapsed;


        List<Item> items = new List<Item>();
        var existCategories = await categoryService.RetrieveAllAsync();
        int Number = 0;
        if (existCategories is not null) 
        {
            foreach (var category in existCategories) 
            {
                items.Add(new Item()
                {
                    Id = category.Id,
                    TartibRaqam = ++Number,
                    Name = category.Name.ToUpper(),
                });
            }
        }
        categoryDataGrid.ItemsSource = items;
    }

    private void rbtnPrice_Checked(object sender, RoutedEventArgs e)
    {
        spProduct.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
    }

    private async void btnCategory_Click(object sender, RoutedEventArgs e)
    {
        spProduct.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Visible;
        spJisNew.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Visible;
        spCategory.Visibility = Visibility.Collapsed;
        grCategoryShow.Visibility = Visibility.Visible;
        grProductShow.Visibility = Visibility.Collapsed;

        List<Item> items = new List<Item>();
        var existCategories = await categoryService.RetrieveAllAsync();
        int Number = 0;
        if (existCategories is not null)
        {
            foreach (var category in existCategories)
            {
                items.Add(new Item()
                {
                    Id = category.Id,
                    TartibRaqam = ++Number,
                    Name = category.Name.ToUpper(),
                });
            }
        }
        categoryDataGrid.ItemsSource = items;
    }

    private void btnQaytish_Click(object sender, RoutedEventArgs e)
    {
        spProduct.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        spCategory.Visibility = Visibility.Visible;
        grCategoryShow.Visibility = Visibility.Collapsed;
        grProductShow.Visibility = Visibility.Visible;
    }

    private async void btnAddCategory_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kategoriyani yaratish uchun DTO obyekti
            CategoryCreationDto categoryCreationDto = new CategoryCreationDto
            {
                Name = txtbCategoryName.Text,
                Description = txtbDescriptionCategory.Text
            };

            // Kategoriya qo'shish uchun xizmat chaqirilyapti
            bool result = await categoryService.AddAsync(categoryCreationDto);
            if (result)
            {
                MessageBox.Show("Kategoriya muvaffaqiyatli saqlandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Kutilmagan xatoliklar uchun
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtbCategoryName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string searchText = txtbCategoryName.Text.Trim();
        FilterCategories(searchText);
    }

    private void txtbName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string searchText = txtbName.Text.Trim();
        FilterProducts(searchText);
    }
    
    private async void FilterCategories(string searchText)
    {
        List<Item> items = new List<Item>();
        var categories =await categoryService.RetrieveAllAsync();

        if (categories is not null)
        {
            int Number = 0;
            categoryDataGrid.ItemsSource = string.Empty;
            foreach (var category in categories)
            {
                if (category.Name.Contains(searchText.ToLower()))
                        items.Add(new Item()
                        {
                            Id = category.Id,
                            TartibRaqam = ++Number,
                            Name = category.Name.ToUpper()
                        }); 
            }
            categoryDataGrid.ItemsSource = items;
        }
    }

    private async void FilterProducts(string searchText)
    {
        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

        if (products is not null)
        {
            int Number = 0;
            productDataGrid.ItemsSource = string.Empty;
            foreach (var product in products)
            {
                if (product.Name.Contains(searchText.ToLower()))
                    items.Add(new ItemProduct()
                    {
                        Id = product.Id,
                        TartibRaqam = ++Number,
                        Name = product.Name.ToUpper()
                    });

            }
            productDataGrid.ItemsSource = items;
        }
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        
        if (rbtnCategory.IsChecked.HasValue.Equals(true) && spQaytish.Visibility != Visibility.Visible )
        {
            return;
        }
        if (categoryDataGrid.SelectedItem is Item selectedUser)
            CustomerInfo.CategoryId = selectedUser.Id;

        spProduct.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        spCategory.Visibility = Visibility.Visible;
        grCategoryShow.Visibility = Visibility.Collapsed;
        grProductShow.Visibility = Visibility.Visible;
    }

    private async void btnProductAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ProductCreationDto productCreationDto = new ProductCreationDto();

            if (txtbName.Text.Equals("") || txtbDescription.Text.Equals(""))
                MessageBox.Show("Ma'lumotni to'liq kiriting.");

            productCreationDto.Name = txtbName.Text.Trim();
            productCreationDto.Description = txtbDescription.Text.Trim();
            productCreationDto.IsActive = true;

            productCreationDto.CategoryId = CustomerInfo.CategoryId;
            if (productCreationDto.CategoryId.Equals(0))
            {
                MessageBox.Show("Kategoriyani tanlang.");
                return;
            }

            bool result = await productService.AddAsync(productCreationDto);
            if (result)
            {
                MessageBox.Show("Kategoriya muvaffaqiyatli saqlandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Kutilmagan xatoliklar uchun
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
}
