using System.Windows;
using DeLong_Desktop.Pages.Products;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.DTOs.Category;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Products;

/// <summary>
/// Interaction logic for ProductEditWindow.xaml
/// </summary>
public partial class ProductEditWindow : Window
{
    private readonly ICategoryService categoryService;
    private readonly IProductService productService;
    private readonly IPriceService priceService;
    private readonly IServiceProvider services;

    public ProductEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();
    }

    private async void rbtnProduct_Click(object sender, RoutedEventArgs e)
    {
        spCategory.Visibility = Visibility.Visible;
        spProduct.Visibility = Visibility.Visible;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        grCategoryShow.Visibility = Visibility.Hidden;

        var product = await productService.RetrieveByIdAsync(ProductInfo.ProductId);

        txtbName.Text = product.Name.ToUpper();
        txtbDescription.Text = product.Description.ToUpper();
    }

    private async void rbtnCategory_Click(object sender, RoutedEventArgs e)
    {
        spCategory.Visibility = Visibility.Visible;
        spProduct.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Visible;
        spJisNew.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        grCategoryShow.Visibility = Visibility.Collapsed;

        var category = await categoryService.RetrieveByIdAsync(ProductInfo.CategoryId);

        txtbCategoryName.Text = category.Name.ToUpper();
        txtbDescriptionCategory.Text = category.Description.ToUpper();
    }

    private async void btnCategory_Click(object sender, RoutedEventArgs e)
    {
        spCategory.Visibility = Visibility.Collapsed;
        spProduct.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Visible;
        spJisNew.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Visible;
        grCategoryShow.Visibility = Visibility.Visible;

        var existCategory = await categoryService.RetrieveByIdAsync(ProductInfo.CategoryId);

        txtbCategoryName.Text = existCategory.Name.ToUpper();
        txtbDescriptionCategory.Text = existCategory.Description.ToUpper();

        // categoryDatagrid malumotlar bilan to'ldiryapti
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
        spCategory.Visibility = Visibility.Visible;
        spProduct.Visibility = Visibility.Visible;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        spQaytish.Visibility = Visibility.Collapsed;
        grCategoryShow.Visibility = Visibility.Collapsed;
    }

    private void txtbCategoryName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string searchText = txtbCategoryName.Text.Trim();
        FilterCategories(searchText);
    }
    private async void FilterCategories(string searchText)
    {
        List<Item> items = new List<Item>();
        var categories = await categoryService.RetrieveAllAsync();

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

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {

        if (rbtnCategory.IsChecked.HasValue.Equals(true) && spQaytish.Visibility != Visibility.Visible)
        {
            return;
        }
        if (categoryDataGrid.SelectedItem is Item selectedUser)
            ProductInfo.CategoryId = selectedUser.Id;

        spProduct.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Collapsed;
        spCategoryPanel.Visibility = Visibility.Collapsed;
        spJisNew.Visibility = Visibility.Collapsed;
        spCategory.Visibility = Visibility.Visible;
        grCategoryShow.Visibility = Visibility.Collapsed;
    }

    private async void btnProductEdit_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (txtbName.Text.Equals("") || txtbDescription.Text.Equals(""))
            {
                MessageBox.Show("Malumotni to'liq kiriting!");
                return;
            }

            ProductUpdateDto productUpdateDto = new ProductUpdateDto
            {
                Id = ProductInfo.ProductId,
                Name = txtbName.Text.ToLower(),
                Description = txtbDescription.Text.ToLower(),
                CategoryId = ProductInfo.CategoryId,
                IsActive = true
            };

            // Productni yangilash uchun xizmat chaqirilyapti
            bool result = await this.productService.ModifyAsync(productUpdateDto);

            if (result)
            {
                MessageBox.Show("Mahsulot muvaffaqiyatli yangilandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private async void btnEditCategory_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (txtbCategoryName.Text.Equals("") || txtbDescriptionCategory.Text.Equals(""))
            {
                MessageBox.Show("Malumotni to'liq kiriting!");
                return;
            }

            CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto
            {
                Id = ProductInfo.CategoryId,
                Name = txtbCategoryName.Text.ToLower(),
                Description = txtbDescriptionCategory.Text.ToLower()
            };

            // categoriyani yangilash uchun chaqirildi
            bool result = await this.categoryService.ModifyAsync(categoryUpdateDto);

            if (result)
            {
                MessageBox.Show("Kategoriya muvaffaqiyatli yangilandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
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
