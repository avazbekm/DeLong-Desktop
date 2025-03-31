using System;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Windows.Pirces;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.Windows.Products;

namespace DeLong_Desktop.Pages.Input;

public partial class InputPage : Page
{
    private readonly IServiceProvider services;
    private readonly IPriceService priceService;
    private readonly IProductService productService;
    private readonly ICategoryService categoryService;

    public InputPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();

        LoadCategoriesAsync();
        LoadProductsAsync();
        AppState.CurrentInputPage = this; // Saqlash
    }
    private async void LoadCategoriesAsync()
    {
        try
        {
            var categories = await categoryService.RetrieveAllAsync();
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

    public async void LoadProductsAsync()
    {
        productDataGrid.ItemsSource = string.Empty;
        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

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
        productDataGrid.ItemsSource = items;
    }

    private void rbtnCategory_Checked(object sender, RoutedEventArgs e)
    {
        radioColumn.Header = "🔳";
        if (categoryDataGrid.SelectedItem is ItemCategory selectedCategory)
        {
            InputInfo.CategoryId = selectedCategory.Id;
            LoadDataAsync(selectedCategory.Id);
        }
    }

    public async void LoadDataAsync(long categoryId)
    {
        productDataGrid.ItemsSource = null;
        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

        if (products is not null)
        {
            foreach (var product in products)
            {
                if (product.CategoryId == categoryId)
                {
                    items.Add(new ItemProduct()
                    {
                        Id = product.Id,
                        Product = product.Name.ToUpper()
                    });
                }
            }
        }
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
        List<ItemCategory> items = new List<ItemCategory>();
        var categories = await categoryService.RetrieveAllAsync();

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
        productDataGrid.ItemsSource = string.Empty;
        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

        if (products is not null)
        {
            foreach (var product in products)
            {
                if (product.Name.Contains(searchText.ToLower()))
                    items.Add(new ItemProduct()
                    {
                        Id = product.Id,
                        Product = product.Name.ToUpper(),
                    });
            }
        }
        productDataGrid.ItemsSource = items;
    }

    private async void FilterProductsAsync(string searchText, long categoryId)
    {
        productDataGrid.ItemsSource = string.Empty;
        List<ItemProduct> items = new List<ItemProduct>();
        var products = await productService.RetrieveAllAsync();

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
        productDataGrid.ItemsSource = items;
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        rbtnProductHeader.Header = "🔳";
        if (productDataGrid.SelectedItem is ItemProduct product)
        {
            InputInfo.ProductId = product.Id;
            if (product.Id > 0)
            {
                tbProductName.Text = product.Product.ToUpper();
                btnAddPrice.Visibility = Visibility.Visible;
            }
            else
            {
                btnAddPrice.Visibility = Visibility.Collapsed;
                tbProductName.Text = "";
            }
            RefreshPrices(product.Id);
        }
    }

    private async void RefreshPrices(long productId)
    {
        try
        {
            wrpPrice.Children.Clear();
            var existPrices = await priceService.RetrieveAllAsync(productId);

            if (existPrices is not null && existPrices.Any())
            {
                foreach (var price in existPrices)
                {
                    PriceViewControl priceViewControl = new PriceViewControl(services)
                    {
                        tbIncomePrice = { Text = price.CostPrice.ToString() },
                        tbSellPrice = { Text = price.SellingPrice.ToString() },
                        tbQuantity = { Text = price.Quantity.ToString() },
                        tbUnitOfMesure = { Text = price.UnitOfMeasure },
                        tbPriceId = { Text = price.Id.ToString() }
                    };
                    priceViewControl.PriceUpdated += (s, e) => RefreshPrices(InputInfo.ProductId);
                    wrpPrice.Children.Add(priceViewControl);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error retrieving prices: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAddPrice_Click(object sender, RoutedEventArgs e)
    {
        PirceWindow pirceWindow = new PirceWindow(services);
        pirceWindow.PriceAdded += (s, e) => RefreshPrices(InputInfo.ProductId);
        pirceWindow.ShowDialog();
    }

    private void categoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void btnAddProduct_Click(object sender, RoutedEventArgs e)
    {
        ProductAddWindow productAddWindow = new ProductAddWindow(services);
        productAddWindow.ProductAdded += (s, ev) => LoadProductsAsync(); // Hodisani ushlaymiz
        productAddWindow.ShowDialog();
    }
}