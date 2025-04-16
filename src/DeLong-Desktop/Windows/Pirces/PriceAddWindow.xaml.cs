using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Companents;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Suppliers;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

public partial class PriceAddWindow : Window
{
    private readonly IServiceProvider _services;
    private readonly ISupplierService _supplierService;
    public bool IsModified { get; set; } = false;
    public event EventHandler PriceModified;

    public PriceAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _supplierService = services.GetRequiredService<ISupplierService>();
        tbQuantity.Focus();
        LoadSuppliersAsync();
    }

    private async void LoadSuppliersAsync()
    {
        try
        {
            var suppliers = await _supplierService.RetrieveAllAsync() ?? new List<SupplierResultDto>();
            var supplierItems = new List<object> { new { Id = (long?)0, Name = "Tanlanmagan" } };
            supplierItems.AddRange(suppliers.Select(s => new { Id = (long?)s.Id, Name = s.Name }));
            cbSupplier.ItemsSource = supplierItems;
            cbSupplier.DisplayMemberPath = "Name";
            cbSupplier.SelectedValuePath = "Id";
            cbSupplier.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yetkazib beruvchilarni yuklashda xato: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!decimal.TryParse(tbQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Miqdor 0 dan katta raqam bo‘lishi kerak.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSupplier = cbSupplier.SelectedItem as dynamic;
            long? supplierId = selectedSupplier?.Id;
            if (supplierId == 0 || !supplierId.HasValue)
            {
                MessageBox.Show("Yetkazib beruvchi tanlanmagan.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PriceInfo.PriceId <= 0)
            {
                MessageBox.Show("Narx ID si aniqlanmadi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var inputPage = AppState.CurrentInputPage;
            if (inputPage != null)
            {
                var product = inputPage.productDataGrid.SelectedItem as ItemProduct;
                inputPage._receiveItems.Add(new ReceiveItem
                {
                    ProductId = InputInfo.ProductId,
                    ProductName = product?.Product ?? "Noma‘lum",
                    Quantity = quantity,
                    UnitOfMeasure = PriceInfo.UnitOfMesure,
                    CostPrice = PriceInfo.ArrivalPrice,
                    TotalAmount = PriceInfo.ArrivalPrice * quantity,
                    SupplierId = supplierId.Value,
                    SellingPrice = PriceInfo.SellingPrice,
                    IsUpdate = true,
                    PriceId = PriceInfo.PriceId
                });
                inputPage.RefreshReceiveDataGrid();
            }

            IsModified = true;
            PriceModified?.Invoke(this, EventArgs.Empty);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null)
        {
            string currentText = textBox.Text;
            string filteredText = "";
            bool hasDot = false;

            foreach (char c in currentText)
            {
                if (char.IsDigit(c))
                {
                    filteredText += c;
                }
                else if (c == '.' && !hasDot)
                {
                    filteredText += c;
                    hasDot = true;
                }
            }

            if (currentText != filteredText)
            {
                int caretIndex = textBox.CaretIndex - (currentText.Length - filteredText.Length);
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Max(0, caretIndex);
            }
        }
    }
}