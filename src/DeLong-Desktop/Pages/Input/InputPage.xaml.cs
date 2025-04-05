using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Windows.Pirces;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.DTOs.Category;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Transactions;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.Pages.Input;

public partial class InputPage : Page
{
    public readonly List<ReceiveItem> _receiveItems = new();
    private readonly IServiceProvider _services;
    private readonly IPriceService _priceService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ITransactionService _transactionService;
    private readonly ITransactionItemService _transactionItemService;

    public InputPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _priceService = services.GetRequiredService<IPriceService>();
        _productService = services.GetRequiredService<IProductService>();
        _categoryService = services.GetRequiredService<ICategoryService>();
        _transactionService = services.GetRequiredService<ITransactionService>();
        _transactionItemService = services.GetRequiredService<ITransactionItemService>();

        LoadCategoriesAsync();
        LoadProductsAsync();
        AppState.CurrentInputPage = this;
        receiveDataGrid.ItemsSource = _receiveItems;
    }

    // Kategoriyalarni yuklash
    public async void LoadCategoriesAsync()
    {
        try
        {
            var categories = await _categoryService.RetrieveAllAsync() ?? new List<CategoryResultDto>();
            var comboBoxItems = new List<ItemCategory> { new() { Id = 0, Category = "Kategoriya tanlang" } };
            comboBoxItems.AddRange(categories.Select(category => new ItemCategory
            {
                Id = category.Id,
                Category = category.Name.ToUpper()
            }));

            cbxCategory.ItemsSource = comboBoxItems;
            cbxCategory.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriyalarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Mahsulotlarni yuklash
    public async void LoadProductsAsync()
    {
        await LoadProductsAsync(null);
    }

    // Kategoriyaga qarab mahsulotlarni yuklash
    public async void LoadDataAsync(long categoryId)
    {
        await LoadProductsAsync(categoryId);
    }

    private async Task LoadProductsAsync(long? categoryId)
    {
        try
        {
            productDataGrid.ItemsSource = null;
            var products = await _productService.RetrieveAllAsync() ?? new List<ProductResultDto>();
            var items = products
                .Where(p => !categoryId.HasValue || p.CategoryId == categoryId.Value)
                .Select(p => new ItemProduct
                {
                    Id = p.Id,
                    Product = p.Name.ToUpper()
                })
                .ToList();

            productDataGrid.ItemsSource = items;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void cbxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxCategory.SelectedValue is long selectedId)
        {
            InputInfo.CategoryId = selectedId;
            if (selectedId == 0)
                LoadProductsAsync(); // Barcha mahsulotlar
            else
                LoadDataAsync(selectedId); // Tanlangan kategoriya mahsulotlari
        }
    }

    private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtProductSearch.Text.Trim();
        FilterProductsAsync(searchText, InputInfo.CategoryId == 0 ? null : InputInfo.CategoryId);
    }

    private async void FilterProductsAsync(string searchText, long? categoryId)
    {
        try
        {
            productDataGrid.ItemsSource = null;
            var products = await _productService.RetrieveAllAsync() ?? new List<ProductResultDto>();
            var items = products
                .Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) &&
                            (!categoryId.HasValue || p.CategoryId == categoryId.Value))
                .Select(p => new ItemProduct
                {
                    Id = p.Id,
                    Product = p.Name.ToUpper()
                })
                .ToList();

            productDataGrid.ItemsSource = items;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlarni filtrlashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        rbtnProductHeader.Header = "🔳";
        if (productDataGrid.SelectedItem is ItemProduct product && product.Id > 0)
        {
            InputInfo.ProductId = product.Id;
            tbProductName.Text = product.Product.ToUpper();
            btnAddPrice.Visibility = Visibility.Visible;
            RefreshPrices(product.Id);
        }
        else
        {
            InputInfo.ProductId = 0;
            btnAddPrice.Visibility = Visibility.Collapsed;
            tbProductName.Text = string.Empty;
            wrpPrice.Children.Clear();
        }
    }

    private async void RefreshPrices(long productId)
    {
        try
        {
            wrpPrice.Children.Clear();
            var existPrices = await _priceService.RetrieveAllAsync(productId) ?? new List<PriceResultDto>();
            foreach (var price in existPrices)
            {
                var priceViewControl = new PriceViewControl(_services)
                {
                    tbIncomePrice = { Text = price.CostPrice.ToString() },
                    tbSellPrice = { Text = price.SellingPrice.ToString() },
                    tbQuantity = { Text = price.Quantity.ToString() },
                    tbUnitOfMesure = { Text = price.UnitOfMeasure },
                    tbPriceId = { Text = price.Id.ToString() }
                };
                priceViewControl.PriceUpdated += (s, ev) => RefreshPrices(InputInfo.ProductId);
                wrpPrice.Children.Add(priceViewControl);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Narxlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAddPrice_Click(object sender, RoutedEventArgs e)
    {
        if (InputInfo.ProductId <= 0 || productDataGrid.SelectedItem is not ItemProduct) return;

        var priceWindow = new PirceWindow(_services);
        priceWindow.PriceAdded += (s, ev) => RefreshPrices(InputInfo.ProductId);
        priceWindow.ShowDialog();
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ReceiveItem item)
        {
            _receiveItems.Remove(item);
            RefreshReceiveDataGrid();
        }
    }

    public void RefreshReceiveDataGrid()
    {
        receiveDataGrid.ItemsSource = null;
        receiveDataGrid.ItemsSource = _receiveItems;
    }

    private async void btnFinalize_Click(object sender, RoutedEventArgs e)
    {
        if (!_receiveItems.Any())
        {
            MessageBox.Show("Hech qanday mahsulot qo‘shilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var transactionDto = new TransactionCreationDto
            {
                SupplierIdFrom = InputInfo.SupplierId,
                BranchId = InputInfo.BranchId,
                BranchIdTo = InputInfo.BranchId,
                TransactionType = TransactionType.Kirim,
                Comment = "Yetkazib beruvchidan mahsulot keldi.",
                Items = _receiveItems.Select(item => new TransactionItemCreationDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    PriceProduct = item.CostPrice,
                }).ToList()
            };

            var transaction = await _transactionService.AddAsync(transactionDto);

            MessageBox.Show("Tranzaksiya muvaffaqiyatli yakunlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            _receiveItems.Clear();
            RefreshReceiveDataGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tranzaksiya yakunlashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}