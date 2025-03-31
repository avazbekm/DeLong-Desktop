using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using DeLong_Desktop.Windows.Suppliers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Suppliers;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Suppliers;

public partial class SuppliersPage : Page
{
    private readonly ISupplierService _supplierService;
    private List<SupplierResultDto> allSuppliers;
    private readonly IServiceProvider services;

    public SuppliersPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _supplierService = services.GetRequiredService<ISupplierService>();
        LoadSuppliers();
    }

    private async void LoadSuppliers()
    {
        try
        {
            var suppliers = await _supplierService.RetrieveAllAsync();
            // Har bir taminotchi nomini katta harflarga aylantirish
            allSuppliers = suppliers.Select(s => new SupplierResultDto
            {
                Id = s.Id,
                Name = s.Name.ToUpper(),        // Katta harflarga o‘zgartirish
                ContactInfo = s.ContactInfo
            }).ToList();
            userDataGrid.ItemsSource = allSuppliers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Taminotchilarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new AddSupplierWindow(services);
        if (addWindow.ShowDialog() == true)
        {
            LoadSuppliers(); // Yangi taminotchi qo‘shilgandan so‘ng DataGrid’ni yangilash
        }
    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        ExportToExcel();
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtSearch.Text.ToLower();
        var filteredSuppliers = allSuppliers.Where(s =>
            s.Name.ToLower().Contains(searchText) || // Qidiruvda katta-kichik harf sezgir emas
            s.ContactInfo.ToLower().Contains(searchText)).ToList();
        userDataGrid.ItemsSource = filteredSuppliers;
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is SupplierResultDto supplier)
        {
            var editWindow = new EditSupplierWindow(_supplierService, supplier);
            if (editWindow.ShowDialog() == true)
            {
                LoadSuppliers(); // Tahrirlangandan so‘ng DataGrid’ni yangilash
            }
        }
    }

    private async void Delete_Button_Click(object sender, RoutedEventArgs e) // async qo‘shildi
    {
        if (sender is Button button && button.DataContext is SupplierResultDto supplier)
        {
            var result = MessageBox.Show($"Taminotchi '{supplier.Name}' ni o‘chirishni xohlaysizmi?", "O‘chirish", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    button.IsEnabled = false; // Tugmani vaqtincha o‘chirish
                    var success = await _supplierService.RemoveAsync(supplier.Id); // await bilan chaqirish
                    if (success)
                    {
                        LoadSuppliers(); // O‘chirilgandan so‘ng DataGrid’ni yangilash
                        MessageBox.Show("Taminotchi muvaffaqiyatli o‘chirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("O‘chirishda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    button.IsEnabled = true; // Tugmani qayta faollashtirish
                }
            }
        }
    }

    private void ExportToExcel()
    {
        if (allSuppliers == null || !allSuppliers.Any())
        {
            MessageBox.Show("Eksport qilish uchun ma'lumotlar yo‘q!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Taminotchilar");

            worksheet.Cell(1, 1).Value = "Taminotchi nomi";
            worksheet.Cell(1, 2).Value = "Telefon";
            worksheet.Range("A1:B1").Style.Font.Bold = true;

            int row = 2;
            foreach (var supplier in allSuppliers)
            {
                worksheet.Cell(row, 1).Value = supplier.Name; // Bu yerda allaqachon katta harflar
                worksheet.Cell(row, 2).Value = supplier.ContactInfo;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = $"Taminotchilar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excel fayliga muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}