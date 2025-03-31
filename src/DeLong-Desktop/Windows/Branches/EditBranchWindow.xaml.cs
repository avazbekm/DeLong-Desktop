using System.Windows;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Branchs;

namespace DeLong_Desktop.Windows.Branches;

public partial class EditBranchWindow : Window
{
    private readonly IBranchService _branchService;
    private readonly BranchResultDto _branch;

    public EditBranchWindow(IBranchService branchService, BranchResultDto branch)
    {
        InitializeComponent();
        _branchService = branchService;
        _branch = branch;

        txtBranchName.Text = branch.BranchName;
        txtLocation.Text = branch.Location;
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
            var dto = new BranchUpdateDto
            {
                Id = _branch.Id,
                BranchName = branchName,
                Location = location
            };

            var result = await _branchService.ModifyAsync(dto);
            if (result != null)
            {
                MessageBox.Show("Filial muvaffaqiyatli yangilandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Filial yangilashda xatolik yuz berdi!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
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