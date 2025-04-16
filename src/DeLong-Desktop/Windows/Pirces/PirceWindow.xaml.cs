using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

public partial class PirceWindow : Window
{
    private readonly IServiceProvider _services;
    private readonly ISupplierService _supplierService;
    public event EventHandler PriceAdded;

    public PirceWindow(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _supplierService = services.GetRequiredService<ISupplierService>();
        LoadSuppliersAsync();
    }

    private async void LoadSuppliersAsync()
    {
        try
        {
            var supplierItems = new List<object> { new { Id = (long?)null, Name = "Tanlanmagan" } };
            var suppliers = await _supplierService.RetrieveAllAsync();
            if (suppliers != null)
            {
                supplierItems.AddRange(suppliers.Select(s => new { Id = (long?)s.Id, Name = s.Name }));
            }
            cbSupplier.ItemsSource = supplierItems;
            cbSupplier.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yetkazib beruvchilarni yuklashda xato: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void cbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbSupplier.SelectedItem != null)
        {
            var selectedItem = cbSupplier.SelectedItem as dynamic;
            long? selectedId = selectedItem?.Id;
            if (!selectedId.HasValue)
            {
                cbSupplier.SelectedIndex = -1;
            }
        }
    }

    private void tbIncomePrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterNumericInput(sender as TextBox);
    }

    private void tbSellPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterNumericInput(sender as TextBox);
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterNumericInput(sender as TextBox);
    }

    private void FilterNumericInput(TextBox textBox)
    {
        if (textBox == null) return;
        int selectionStart = textBox.SelectionStart;
        string filteredText = string.Empty;
        bool decimalPointSeen = false;

        foreach (char c in textBox.Text)
        {
            if (char.IsDigit(c)) filteredText += c;
            else if (c == '.' && !decimalPointSeen)
            {
                filteredText += c;
                decimalPointSeen = true;
            }
        }

        if (textBox.Text != filteredText)
        {
            textBox.Text = filteredText;
            textBox.SelectionStart = Math.Min(selectionStart, filteredText.Length);
        }
    }

    private void btnAddPrice_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(tbIncomePrice.Text) ||
                string.IsNullOrEmpty(tbSellPrice.Text) ||
                string.IsNullOrEmpty(tbQuantity.Text) ||
                string.IsNullOrEmpty(tbUnitOfMesure.Text))
            {
                MessageBox.Show("Ma'lumotlarni to'liq kiriting.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(tbQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Miqdor 0 dan katta bo‘lishi kerak.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(tbIncomePrice.Text, out decimal costPrice) || costPrice <= 0)
            {
                MessageBox.Show("Kelish narxi 0 dan katta bo‘lishi kerak.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(tbSellPrice.Text, out decimal sellingPrice) || sellingPrice <= 0)
            {
                MessageBox.Show("Sotish narxi 0 dan katta bo‘lishi kerak.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSupplier = cbSupplier.SelectedItem as dynamic;
            long? supplierId = selectedSupplier?.Id;
            if (!supplierId.HasValue)
            {
                MessageBox.Show("Yetkazib beruvchi tanlanmagan.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var inputPage = AppState.CurrentInputPage;
            if (inputPage != null)
            {
                var product = inputPage.productDataGrid.SelectedItem as ItemProduct;
                inputPage._receiveItems.Add(new ReceiveItem
                {
                    ProductId = InputInfo.ProductId,
                    ProductName = product?.Product ?? "Noma'lum",
                    Quantity = quantity,
                    UnitOfMeasure = tbUnitOfMesure.Text,
                    CostPrice = costPrice,
                    TotalAmount = costPrice * quantity,
                    SupplierId = supplierId.Value,
                    SellingPrice = sellingPrice,
                    IsUpdate = false,
                    PriceId = 0
                });
                inputPage.RefreshReceiveDataGrid();
            }

            tbIncomePrice.Text = "";
            tbSellPrice.Text = "";
            tbQuantity.Text = "";
            tbUnitOfMesure.Text = "";
            cbSupplier.SelectedIndex = 0;

            PriceAdded?.Invoke(this, EventArgs.Empty);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        PriceAdded?.Invoke(this, EventArgs.Empty);
    }
}