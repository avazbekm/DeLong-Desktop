using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.ApiService.DTOs.Transactions;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;
using DeLong_Desktop.ApiService.DTOs.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Pirces;

public partial class PirceWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IProductService productService;
    private readonly ICategoryService categoryService;
    private readonly IPriceService priceService;
    private readonly IBranchService branchService;
    private readonly ISupplierService supplierService;
    private readonly ITransactionService transactionService;
    private readonly ITransactionItemService transactionItemService;
    public bool IsCreated { get; set; } = false;
    public event EventHandler PriceAdded;
    private long? CurrentTransactionId = null;
    private List<TransactionItemCreationDto> TransactionItems = new List<TransactionItemCreationDto>();

    public PirceWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        categoryService = services.GetRequiredService<ICategoryService>();
        productService = services.GetRequiredService<IProductService>();
        priceService = services.GetRequiredService<IPriceService>();
        branchService = services.GetRequiredService<IBranchService>();
        supplierService = services.GetRequiredService<ISupplierService>();
        transactionService = services.GetRequiredService<ITransactionService>();
        transactionItemService = services.GetRequiredService<ITransactionItemService>();

        LoadSuppliersAndBranches();
        LoadReceivers();
    }

    private async void LoadSuppliersAndBranches()
    {
        try
        {
            var supplierItems = new List<object> { new { Id = (long?)null, Name = "Tanlanmagan" } };
            var branches = await branchService.RetrieveAllAsync();
            if (branches != null)
            {
                supplierItems.AddRange(branches.Select(b => new { Id = (long?)b.Id, Name = b.BranchName }));
            }
            var suppliers = await supplierService.RetrieveAllAsync();
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
            var branches = await branchService.RetrieveAllAsync();
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

    private async void cbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbSupplier.SelectedItem != null)
        {
            var selectedItem = cbSupplier.SelectedItem as dynamic;
            long? selectedId = selectedItem?.Id;

            if (selectedId.HasValue)
            {
                long currentBranchId = await GetCurrentUserBranchIdAsync();
                cbReceiver.SelectedItem = cbReceiver.Items.Cast<dynamic>().FirstOrDefault(item => item.Id == currentBranchId);

                if (CurrentTransactionId == null)
                {
                    await CreateNewTransactionAsync(selectedId.Value);
                }
            }
            else
            {
                cbReceiver.SelectedIndex = -1;
                CurrentTransactionId = null;
                TransactionItems.Clear();
            }
        }
    }

    private async Task CreateNewTransactionAsync(long supplierId)
    {
        try
        {
            var transactionDto = new TransactionCreationDto
            {
                SupplierIdFrom = supplierId,
                BranchId = await GetCurrentUserBranchIdAsync(),
                BranchIdTo = ((dynamic)cbReceiver.SelectedItem)?.Id,
                TransactionType = TransactionType.Kirim,
                Comment = "Yetkazib beruvchidan mahsulot keldi"
            };
            var result = await transactionService.AddAsync(transactionDto);
            if (result != null)
            {
                CurrentTransactionId = result.Id;
            }
            else
            {
                MessageBox.Show("Tranzaksiya yaratishda xatolik yuz berdi.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tranzaksiya yaratishda xato: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task<long> GetCurrentUserBranchIdAsync()
    {
        var branches = await branchService.RetrieveAllAsync();
        return branches?.FirstOrDefault()?.Id ?? 1; // Haqiqiy logikaga almashtiriladi
    }

    private void tbIncomePrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
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
    }

    private void tbSellPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
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
    }

    private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
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
                MessageBox.Show("Ma'lumotlarni to'liq kiriting.");
                return;
            }

            if (CurrentTransactionId == null)
            {
                MessageBox.Show("Avval yetkazib beruvchini tanlang.");
                return;
            }

            PriceCreationDto priceCreationDto = new PriceCreationDto()
            {
                CostPrice = decimal.Parse(tbIncomePrice.Text),
                SellingPrice = decimal.Parse(tbSellPrice.Text),
                Quantity = decimal.Parse(tbQuantity.Text),
                UnitOfMeasure = tbUnitOfMesure.Text,
                ProductId = InputInfo.ProductId
            };
            await priceService.AddAsync(priceCreationDto);

            var transactionItemDto = new TransactionItemCreationDto
            {
                TransactionId = CurrentTransactionId.Value,
                ProductId = InputInfo.ProductId,
                Quantity = decimal.Parse(tbQuantity.Text),
                UnitOfMeasure = tbUnitOfMesure.Text,
                PriceProduct = decimal.Parse(tbIncomePrice.Text)
            };
            TransactionItems.Add(transactionItemDto);

            tbIncomePrice.Text = "";
            tbSellPrice.Text = "";
            tbQuantity.Text = "";
            tbUnitOfMesure.Text = "";

            MessageBox.Show($"Mahsulot qo'shildi. Hozirgi mahsulotlar soni: {TransactionItems.Count}.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

            if (TransactionItems.Count >= 10)
            {
                await FinalizeTransaction();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task FinalizeTransaction()
    {
        try
        {
            foreach (var item in TransactionItems)
            {
                await transactionItemService.AddAsync(item);
            }
            MessageBox.Show("Tranzaksiya muvaffaqiyatli yakunlandi.", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            IsCreated = true;
            PriceAdded?.Invoke(this, EventArgs.Empty);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tranzaksiyani yakunlashda xato: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (TransactionItems.Any() && CurrentTransactionId.HasValue)
        {
            var result = MessageBox.Show("Tranzaksiya yakunlanmadi. Mahsulotlarni saqlab yakunlashni xohlaysizmi?", "Eslatma", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                FinalizeTransaction().Wait();
            }
        }
        base.OnClosing(e);
    }
}