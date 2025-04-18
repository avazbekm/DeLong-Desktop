using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Suppliers;

namespace DeLong_Desktop.Pages.InputHistories;

/// <summary>
/// Interaction logic for HistoryPage.xaml
/// </summary>
public partial class HistoryPage : Page
{
    private readonly IServiceProvider _services;
    private readonly ICreditorDebtService _creditorDebtService;
    private readonly ICreditorDebtPaymentService _creditorDebtPaymentService;
    private readonly ISupplierService _supplierService;
    private readonly ITransactionItemService _transactionItemService;
    private readonly IUserService _userService;
    private List<HistoryItem> _historyItems = new();

    public HistoryPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _creditorDebtService = services.GetRequiredService<ICreditorDebtService>();
        _creditorDebtPaymentService = services.GetRequiredService<ICreditorDebtPaymentService>();
        _supplierService = services.GetRequiredService<ISupplierService>();
        _transactionItemService = services.GetRequiredService<ITransactionItemService>();
        _userService = services.GetRequiredService<IUserService>();
        AppState.CurrentHistoryPage = this;

        LoadSuppliersAsync();
        LoadHistoryAsync();
    }

    public async void LoadHistoryAsync()
    {
        try
        {
            var debts = await _creditorDebtService.RetrieveAllAsync();
            _historyItems = debts.Select(d => new HistoryItem
            {
                Id = d.Id,
                SupplierId = d.SupplierId,
                SupplierName = d.SupplierName.ToUpper(),
                Date = d.Date,
                TotalAmount = d.RemainingAmount + d.CreditorDebtPayments.Sum(p => p.Amount),
                PaidAmount = d.CreditorDebtPayments.Sum(p => p.Amount),
                RemainingAmount = d.RemainingAmount,
                Status = d.IsSettled ? "To‘langan" : d.CreditorDebtPayments.Any() ? "Qisman to‘langan" : "To‘lanmagan"
            })
            .OrderByDescending(d => d.Date)
            .ToList();

            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tarixni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LoadSuppliersAsync()
    {
        try
        {
            var suppliers = await _supplierService.RetrieveAllAsync() ?? new List<SupplierResultDto>();
            var supplierItems = new List<SupplierItem> { new SupplierItem { Id = 0, Name = "Hammasi" } };
            supplierItems.AddRange(suppliers.Select(s => new SupplierItem { Id = s.Id, Name = s.Name }));

            cbxSupplier.ItemsSource = supplierItems;
            cbxSupplier.SelectedIndex = 0;

            cbxStatus.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Value = "Hammasi" },
                new StatusItem { Value = "To‘langan" },
                new StatusItem { Value = "Qisman to‘langan" },
                new StatusItem { Value = "To‘lanmagan" }
            };
            cbxStatus.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Taminotchilarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFilters()
    {
        var filteredItems = _historyItems.AsEnumerable();

        // Qidiruv
        var searchText = txtSearch.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(searchText))
        {
            filteredItems = filteredItems.Where(i => i.Id.ToString().Contains(searchText) || i.SupplierName.ToLower().Contains(searchText));
        }

        // Sana filtri
        if (dpStartDate.SelectedDate.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.Date.Date >= dpStartDate.SelectedDate.Value);
        }
        if (dpEndDate.SelectedDate.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.Date.Date <= dpEndDate.SelectedDate.Value);
        }

        // Taminotchi filtri
        if (cbxSupplier.SelectedItem is SupplierItem supplier && supplier.Id > 0)
        {
            filteredItems = filteredItems.Where(i => i.SupplierId == supplier.Id);
        }

        // Holat filtri
        if (cbxStatus.SelectedItem is StatusItem status && status.Value != "Hammasi")
        {
            filteredItems = filteredItems.Where(i => i.Status == status.Value);
        }

        historyDataGrid.ItemsSource = filteredItems.ToList();
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void Filter_Changed(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilters();
    }

    private void DetailsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is HistoryItem item)
        {
            try
            {
                // Yangi oynani ochish
                var detailsWindow = new DetailsWindow(item, _creditorDebtService, _creditorDebtPaymentService, _transactionItemService,_userService);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tafsilotlarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void historyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}
