using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DeLong_Desktop.Windows.Employees;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Employees;

/// <summary>
/// Interaction logic for EmployeesPage.xaml
/// </summary>
public partial class EmployeesPage : Page
{
    private readonly IServiceProvider _services;
    private readonly IUserService _userService;
    private ObservableCollection<UserResultDto> _employees;

    public EmployeesPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _userService = services.GetRequiredService<IUserService>();
        _employees = new ObservableCollection<UserResultDto>();
        employeeDataGrid.ItemsSource = _employees;
        LoadEmployeesAsync();
    }

    private async void LoadEmployeesAsync()
    {
        try
        {
            var employees = await _userService.RetrieveAllAsync();
            _employees.Clear();
            foreach (var employee in employees)
            {
                _employees.Add(employee);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xodimlarni yuklashda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var addEmployeeWindow = new AddEmployeeWindow(_services);
            if (addEmployeeWindow.ShowDialog() == true)
            {
                // Xodim qo‘shilgan, DataGrid ni yangilash
                LoadEmployeesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xodim qo‘shish oynasini ochishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is UserResultDto employee)
            {
                var editEmployeeWindow = new EditEmployeeWindow(_services, employee);
                if (editEmployeeWindow.ShowDialog() == true)
                {
                    // Xodim yangilangan, DataGrid ni yangilash
                    LoadEmployeesAsync();
                }
            }
            else
            {
                MessageBox.Show("Xodimni tanlashda xatolik!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xodim tahrirlash oynasini ochishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string searchText = txtSearch.Text?.Trim().ToLower() ?? string.Empty;
            if (string.IsNullOrEmpty(searchText))
            {
                LoadEmployeesAsync();
                return;
            }

            var filtered = _employees.Where(emp =>
                emp.LastName?.ToLower().Contains(searchText) == true ||
                emp.FirstName?.ToLower().Contains(searchText) == true ||
                emp.Phone?.ToLower().Contains(searchText) == true ||
                emp.Username?.ToLower().Contains(searchText) == true ||
                emp.Role.ToString().ToLower().Contains(searchText) == true
            ).ToList();

            _employees.Clear();
            foreach (var employee in filtered)
            {
                _employees.Add(employee);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Qidiruvda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Excelga o‘tkazish funksiyasi hali implement qilinmagan.", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("O‘chirish funksiyasi hali implement qilinmagan.", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

// String ni bosh harflarga aylantirish uchun Converter
public class UpperCaseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.ToUpper();
        }
        return value?.ToString()?.ToUpper() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // ConvertBack kerak emas, chunki DataGrid faqat o‘qish uchun
        throw new NotImplementedException();
    }
}