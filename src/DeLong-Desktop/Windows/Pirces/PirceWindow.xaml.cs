using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

public partial class PirceWindow : Window
{
    private readonly IServiceProvider _services;
    private readonly IPriceService _priceService;
    private readonly IBranchService _branchService;
    private readonly ISupplierService _supplierService;
    public event EventHandler PriceAdded;

    public PirceWindow(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _priceService = services.GetRequiredService<IPriceService>();
        _branchService = services.GetRequiredService<IBranchService>();
        _supplierService = services.GetRequiredService<ISupplierService>();

        LoadSuppliersAndBranches();
        LoadReceivers();
    }

    private async void LoadSuppliersAndBranches()
    {
        try
        {
            var supplierItems = new List<object> { new { Id = (long?)null, Name = "Tanlanmagan" } };
            var branches = await _branchService.RetrieveAllAsync();
            if (branches != null)
            {
                supplierItems.AddRange(branches.Select(b => new { Id = (long?)b.Id, Name = b.BranchName }));
            }
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

    private async void LoadReceivers()
    {
        try
        {
            var branches = await _branchService.RetrieveAllAsync();
            if (branches != null)
            {
                cbReceiver.ItemsSource = branches.Select(b => new { Id = b.Id, Name = b.BranchName }).ToList();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Qabul qiluvchilarni yuklashda xato: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
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
                cbReceiver.SelectedIndex = -1;
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

    private async void btnAddPrice_Click(object sender, RoutedEventArgs e)
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

            var selectedSupplier = cbSupplier.SelectedItem as dynamic;
            long? supplierId = selectedSupplier?.Id;
            InputInfo.SupplierId = supplierId.Value;
            var selectedReceiver = cbReceiver.SelectedItem as dynamic;
            long? receiverId = selectedReceiver?.Id;
            InputInfo.BranchId = receiverId.Value;

            // Price qo‘shish
            var priceCreationDto = new PriceCreationDto
            {
                SupplierId = supplierId,
                ProductId = InputInfo.ProductId,
                CostPrice = decimal.Parse(tbIncomePrice.Text),
                SellingPrice = decimal.Parse(tbSellPrice.Text),
                UnitOfMeasure = tbUnitOfMesure.Text,
                Quantity = decimal.Parse(tbQuantity.Text)
            };
            await _priceService.AddAsync(priceCreationDto);

            // InputPage’dagi receiveDataGrid ga qo‘shish
            var inputPage = AppState.CurrentInputPage;
            if (inputPage != null)
            {
                var product = inputPage.productDataGrid.SelectedItem as ItemProduct;
                inputPage._receiveItems.Add(new ReceiveItem
                {
                    ProductId = InputInfo.ProductId,
                    ProductName = product?.Product ?? "Noma'lum",
                    Quantity = decimal.Parse(tbQuantity.Text),
                    UnitOfMeasure = tbUnitOfMesure.Text,
                    CostPrice = decimal.Parse(tbIncomePrice.Text),
                    TotalAmount = decimal.Parse(tbIncomePrice.Text) * decimal.Parse(tbQuantity.Text)
                });
                inputPage.RefreshReceiveDataGrid();
            }

            // UI ni tozalash
            tbIncomePrice.Text = "";
            tbSellPrice.Text = "";
            tbQuantity.Text = "";
            tbUnitOfMesure.Text = "";

            MessageBox.Show("Mahsulot qo‘shildi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            PriceAdded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}