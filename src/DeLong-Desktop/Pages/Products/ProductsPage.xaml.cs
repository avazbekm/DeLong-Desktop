using System.Data;
using System.Windows;
using ClosedXML.Excel;
using System.Windows.Data;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Windows.Products;
using Page = System.Windows.Controls.Page;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DeLong_Desktop.Pages.Products;

public partial class ProductsPage : Page
{
    private readonly IProductService productService;
    private readonly ICategoryService categoryService;
    private readonly IPriceService priceService;
    private readonly IServiceProvider services;

    public static event EventHandler ProductAdded;

    public ProductsPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();

        LoadCategoriesAsync();
        LoadData();

        // AppState ga joriy ProductsPage ni saqlash
        AppState.CurrentProductsPage = this;
    }

    public async void LoadCategoriesAsync()
    {
        try
        {
            var categories = await this.categoryService.RetrieveAllAsync();

            var comboBoxItems = new List<ItemCombobox> { new ItemCombobox { Id = 0, Name = "Barcha kategoriyalar" } };
            foreach (var category in categories)
            {
                comboBoxItems.Add(new ItemCombobox
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            cmbCategory.ItemsSource = comboBoxItems;
            cmbCategory.DisplayMemberPath = "Name";
            cmbCategory.SelectedValuePath = "Id";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriyalarni yuklashda xato: {ex.Message}");
        }
    }

    private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbCategory.SelectedValue is long selectedCategoryId)
        {
            dataGrid.ItemsSource = string.Empty;
            if (selectedCategoryId == 0)
                LoadData();
            else
                LoadData(selectedCategoryId);
        }
    }

    public async void LoadData(long categoryId)
    {
        dataGrid.ItemsSource = null;
        List<Item> items = new List<Item>();
        var categories = await categoryService.RetrieveAllAsync();
        var products = await productService.RetrieveAllAsync();
        var prices = await priceService.RetrieveAllAsync();

        if (products is not null)
        {
            foreach (var product in products)
            {
                if (product.CategoryId == categoryId)
                {
                    var category = categories.FirstOrDefault(p => p.Id.Equals(categoryId));
                    var productPrices = prices.Where(p => p.ProductId == product.Id);
                    decimal totalStock = productPrices.Sum(p => p.Quantity);

                    items.Add(new Item()
                    {
                        Id = product.Id,
                        Name = product.Name.ToUpper(),
                        MahsulotBelgisi = product.ProductSign.ToUpper(),
                        Stock = totalStock,
                        Category = category.Name.ToUpper(),
                        IsActive = product.IsActive,
                        CategoryId = category.Id
                    });
                }
            }
        }
        dataGrid.ItemsSource = items;
    }

    private async void LoadData()
    {
        dataGrid.ItemsSource = string.Empty;
        List<Item> items = new List<Item>();
        var categories = await categoryService.RetrieveAllAsync();
        var products = await productService.RetrieveAllAsync();
        var prices = await priceService.RetrieveAllAsync();

        if (products is not null)
        {
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));
                var productPrices = prices.Where(p => p.ProductId == product.Id);
                decimal totalStock = productPrices.Sum(p => p.Quantity);

                items.Add(new Item()
                {
                    Id = product.Id,
                    Name = product.Name.ToUpper(),
                    MahsulotBelgisi = product.ProductSign.ToUpper(),
                    Stock = totalStock,
                    Category = category.Name.ToUpper(),
                    IsActive = product.IsActive,
                    CategoryId = category.Id
                });
            }
        }
        dataGrid.ItemsSource = items;
    }

    public async Task RefreshDataAsync()
    {
        if (cmbCategory.SelectedValue is long selectedCategoryId)
        {
            if (selectedCategoryId == 0)
                LoadData();
            else
                LoadData(selectedCategoryId);
        }
        else
        {
            LoadData();
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        ProductAddWindow productAddWindow = new ProductAddWindow(services);
        productAddWindow.ProductAdded += (s, ev) =>
        {
            if (cmbCategory.SelectedValue is long selectedCategoryId)
                LoadData(selectedCategoryId);
            else
                LoadData();

            ProductAdded?.Invoke(this, EventArgs.Empty);

            if (AppState.CurrentInputPage != null)
            {
                if (cmbCategory.SelectedValue is long categoryId && categoryId != 0)
                    AppState.CurrentInputPage.LoadDataAsync(categoryId);
                else
                    AppState.CurrentInputPage.LoadProductsAsync();
            }
        };
        productAddWindow.ShowDialog();
    }

    private void txtSearch_TextChanged(object sender, RoutedEventArgs e)
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

        List<Item> items = new List<Item>();
        var categories = await categoryService.RetrieveAllAsync();
        var products = await productService.RetrieveAllAsync();
        var prices = await priceService.RetrieveAllAsync();

        if (products is not null)
        {
            if (categoryId == 0)
            {
                foreach (var product in products)
                {
                    if (product.Name.ToLower().Contains(searchText.ToLower()))
                    {
                        var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));
                        var productPrices = prices.Where(p => p.ProductId == product.Id);
                        decimal totalStock = productPrices.Sum(p => p.Quantity);

                        items.Add(new Item()
                        {
                            Id = product.Id,
                            Name = product.Name.ToUpper(),
                            MahsulotBelgisi = product.ProductSign.ToUpper(),
                            Stock = totalStock,
                            Category = category.Name.ToUpper(),
                            IsActive = product.IsActive,
                            CategoryId = category.Id
                        });
                    }
                }
            }
            else
            {
                foreach (var product in products)
                {
                    if (product.Name.ToLower().Contains(searchText.ToLower()) && product.CategoryId == categoryId)
                    {
                        var category = categories.FirstOrDefault(c => c.Id.Equals(product.CategoryId));
                        var productPrices = prices.Where(p => p.ProductId == product.Id);
                        decimal totalStock = productPrices.Sum(p => p.Quantity);

                        items.Add(new Item()
                        {
                            Id = product.Id,
                            Name = product.Name.ToUpper(),
                            MahsulotBelgisi = product.ProductSign.ToUpper(),
                            Stock = totalStock,
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
        if (productEditWindow.IsModified)
        {
            if (cmbCategory.SelectedValue is long selectedCategoryId)
                LoadData(selectedCategoryId);
            else
                LoadData();
        }
    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        DataTable dataTable = new DataTable();
        foreach (var column in dataGrid.Columns)
        {
            dataTable.Columns.Add(column.Header.ToString());
        }

        foreach (var item in dataGrid.Items)
        {
            DataRow row = dataTable.NewRow();
            foreach (var column in dataGrid.Columns)
            {
                if (column is DataGridTextColumn textColumn)
                {
                    Binding binding = textColumn.Binding as Binding;
                    string propertyName = binding?.Path.Path;
                    if (propertyName != null && item != null)
                    {
                        var value = item.GetType().GetProperty(propertyName)?.GetValue(item, null);
                        row[column.Header.ToString()] = value ?? "";
                    }
                }
            }
            dataTable.Rows.Add(row);
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Exported Data");
            worksheet.Cell(1, 1).InsertTable(dataTable);

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Fayl (*.xlsx)|*.xlsx",
                FileName = "Mahsulotlar ro'yxati.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excelga o'tkazildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}