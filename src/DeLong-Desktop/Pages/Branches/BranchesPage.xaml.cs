using System.Windows;
using ClosedXML.Excel;
using System.Windows.Controls;
using DeLong_Desktop.Windows.Branches;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Branchs;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Branches;

public partial class BranchesPage : Page
{
    private readonly IBranchService _branchService;
    private List<BranchResultDto> allBranches;
    private readonly IServiceProvider services;

    public BranchesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        _branchService = services.GetRequiredService<IBranchService>();
        LoadBranches();
    }

    private async void LoadBranches()
    {
        try
        {
            var branches = await _branchService.RetrieveAllAsync();
            // Har bir filial nomini katta harflarga aylantirish
            allBranches = branches.Select(b => new BranchResultDto
            {
                Id = b.Id,
                BranchName = b.BranchName.ToUpper(), // Katta harflarga o‘zgartirish
                Location = b.Location.ToUpper(),
                CreatedAt = b.CreatedAt,
                CreatedBy = b.CreatedBy
            }).ToList();
            branchDataGrid.ItemsSource = allBranches;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Filiallarni yuklashda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new AddBranchWindow(services);
        if (addWindow.ShowDialog() == true)
        {
            LoadBranches(); // Yangi filial qo‘shilgandan so‘ng DataGrid’ni yangilash
        }
    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        ExportToExcel();
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = txtSearch.Text.ToLower();
        var filteredBranches = allBranches.Where(b =>
            b.BranchName.ToLower().Contains(searchText) ||
            b.Location.ToLower().Contains(searchText)).ToList();
        branchDataGrid.ItemsSource = filteredBranches;
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BranchResultDto branch)
        {
            var editWindow = new EditBranchWindow(_branchService, branch);
            if (editWindow.ShowDialog() == true)
            {
                LoadBranches(); // Tahrirlangandan so‘ng DataGrid’ni yangilash
            }
        }
    }

    private async void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BranchResultDto branch)
        {
            var result = MessageBox.Show($"Filial '{branch.BranchName}' ni o‘chirishni xohlaysizmi?", "O‘chirish", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    button.IsEnabled = false; // Tugmani vaqtincha o‘chirish
                    var success = await _branchService.RemoveAsync(branch.Id);
                    if (success)
                    {
                        LoadBranches(); // O‘chirilgandan so‘ng DataGrid’ni yangilash
                        MessageBox.Show("Filial muvaffaqiyatli o‘chirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
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
        if (allBranches == null || !allBranches.Any())
        {
            MessageBox.Show("Eksport qilish uchun ma'lumotlar yo‘q!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Filiallar");

            worksheet.Cell(1, 1).Value = "Filial nomi";
            worksheet.Cell(1, 2).Value = "Filial manzili";
            worksheet.Range("A1:B1").Style.Font.Bold = true;

            int row = 2;
            foreach (var branch in allBranches)
            {
                worksheet.Cell(row, 1).Value = branch.BranchName;
                worksheet.Cell(row, 2).Value = branch.Location;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = $"Filiallar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excel fayliga muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
