using System;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Category;
using DeLong_Desktop.ApiService.DTOs.Products;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Products;

public partial class ProductAddWindow : Window
{
    private readonly ICategoryService categoryService;
    private readonly IProductService productService;
    private readonly IPriceService priceService;
    private readonly IServiceProvider services;
    public bool IsCreated { get; set; } = false;
    public event EventHandler ProductAdded; // Yangi hodisa qo‘shildi

    public ProductAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();
        txtbCategoryName.Focus();
        btnCategory.Focus();
    }

    private async void rbtnProduct_Checked(object sender, RoutedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void rbtnCategory_Checked(object sender, RoutedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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
            if (txtbCategoryName.Text.Length == 0)
            {
                MessageBox.Show("Kategoriya nomini kiriting.");
                return;
            }
            CategoryCreationDto categoryCreationDto = new CategoryCreationDto
            {
                Name = txtbCategoryName.Text,
                Description = txtbDescriptionCategory.Text
            };

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
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtbCategoryName_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtbCategoryName.Text.Trim();
        FilterCategories(searchText);
    }

    private void txtbName_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtbName.Text.Trim();
        FilterProducts(searchText);
    }

    private async void FilterCategories(string searchText)
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void FilterProducts(string searchText)
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        if (rbtnCategory.IsChecked.HasValue.Equals(true) && spQaytish.Visibility != Visibility.Visible)
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

            if (txtbName.Text.Equals(""))
            {
                MessageBox.Show("Ma'lumotni to'liq kiriting.");
                return;
            }

            productCreationDto.Name = txtbName.Text.Trim();
            productCreationDto.Description = txtbDescription.Text.Trim();
            if (txtbStock.Text.Length > 0)
                productCreationDto.MinStockLevel = decimal.Parse(txtbStock.Text);
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
                MessageBox.Show("Mahsulot muvaffaqiyatli saqlandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCreated = true;
                ProductAdded?.Invoke(this, EventArgs.Empty); // Hodisani chaqiramiz
                this.Close();
            }
            else
            {
                MessageBox.Show("Saqlashda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kutilmagan xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtbStock_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);
    }
}