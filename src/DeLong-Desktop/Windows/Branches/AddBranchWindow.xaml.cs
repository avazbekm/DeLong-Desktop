using System.Windows;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Branchs;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Branches;

public partial class AddBranchWindow : Window
{
    private readonly IBranchService _branchService;

    public AddBranchWindow(IServiceProvider services)
    {
        InitializeComponent();
        _branchService = services.GetRequiredService<IBranchService>();
        txtBranchName.Focus();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        string branchName = txtBranchName.Text.Trim();
        string location = txtLocation.Text.Trim();

        if (string.IsNullOrEmpty(branchName) || string.IsNullOrEmpty(location))
        {
            MessageBox.Show("Iltimos, barcha maydonlarni to‘ldiring!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var dto = new BranchCreationDto
            {
                BranchName = branchName,
                Location = location
            };

            var result = await _branchService.AddAsync(dto);
            if (result != null)
            {
                MessageBox.Show("Filial muvaffaqiyatli qo‘shildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Filial qo‘shishda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}