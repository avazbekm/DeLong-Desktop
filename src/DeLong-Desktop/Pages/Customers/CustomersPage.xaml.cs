using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Windows.Customers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Customers;

namespace DeLong_Desktop.Pages.Customers;

/// <summary>
/// Interaction logic for CustomersPage.xaml
/// </summary>
public partial class CustomersPage : Page
{
    private readonly ICustomerService customerService;
    private readonly IServiceProvider services;
    private ObservableCollection<CustomerResultDto> customers;

    public static event EventHandler CustomerAdded;

    public CustomersPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        customerService = services.GetRequiredService<ICustomerService>();
        customers = new ObservableCollection<CustomerResultDto>();
        userDataGrid.ItemsSource = customers;
        LoadCustomersAsync();
    }

    private async void LoadCustomersAsync()
    {
        try
        {
            var allCustomers = await customerService.RetrieveAllAsync();
            customers.Clear();
            foreach (var customer in allCustomers)
            {
                // Ma'lumotlarni katta harflarga aylantirish
                var upperCaseCustomer = new CustomerResultDto
                {
                    Id = customer.Id,
                    CompanyName = customer.CompanyName?.ToUpper() ?? string.Empty,
                    EmployeeName = customer.EmployeeName?.ToUpper() ?? string.Empty,
                    EmployeePhone = customer.EmployeePhone?.ToUpper() ?? string.Empty,
                    ManagerName = customer.ManagerName?.ToUpper() ?? string.Empty,
                    ManagerPhone = customer.ManagerPhone?.ToUpper() ?? string.Empty,
                    YurAddress = customer.YurAddress?.ToUpper() ?? string.Empty,
                    MFO = customer.MFO?.ToUpper() ?? string.Empty,
                    INN = customer.INN,
                    BankAccount = customer.BankAccount?.ToUpper() ?? string.Empty,
                    BankName = customer.BankName?.ToUpper() ?? string.Empty,
                    OKONX = customer.OKONX?.ToUpper() ?? string.Empty,
                    BranchId = customer.BranchId
                };
                customers.Add(upperCaseCustomer);
            }
            userDataGrid.Visibility = Visibility.Visible;
            txtSearch.Text = string.Empty;
            userDataGrid.Items.Refresh(); // UI ni yangilash
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mijozlarni yuklashda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string searchText = txtSearch.Text?.Trim().ToLower() ?? string.Empty;
            if (string.IsNullOrEmpty(searchText))
            {
                userDataGrid.ItemsSource = customers; // To‘liq ro‘yxatni ko‘rsatish
            }
            else
            {
                var filteredCustomers = customers.Where(c =>
                    (c.CompanyName?.ToLower().Contains(searchText) ?? false) ||
                    (c.ManagerName?.ToLower().Contains(searchText) ?? false) ||
                    (c.EmployeeName?.ToLower().Contains(searchText) ?? false) ||
                    (c.YurAddress?.ToLower().Contains(searchText) ?? false)).ToList();

                // Filtlangan ma'lumotlarni katta harflarga aylantirish
                var upperCaseFiltered = filteredCustomers.Select(c => new CustomerResultDto
                {
                    Id = c.Id,
                    CompanyName = c.CompanyName?.ToUpper() ?? string.Empty,
                    EmployeeName = c.EmployeeName?.ToUpper() ?? string.Empty,
                    EmployeePhone = c.EmployeePhone?.ToUpper() ?? string.Empty,
                    ManagerName = c.ManagerName?.ToUpper() ?? string.Empty,
                    ManagerPhone = c.ManagerPhone?.ToUpper() ?? string.Empty,
                    YurAddress = c.YurAddress?.ToUpper() ?? string.Empty,
                    MFO = c.MFO?.ToUpper() ?? string.Empty,
                    INN = c.INN,
                    BankAccount = c.BankAccount?.ToUpper() ?? string.Empty,
                    BankName = c.BankName?.ToUpper() ?? string.Empty,
                    OKONX = c.OKONX?.ToUpper() ?? string.Empty,
                    BranchId = c.BranchId
                }).ToList();

                userDataGrid.ItemsSource = new ObservableCollection<CustomerResultDto>(upperCaseFiltered);
            }
            userDataGrid.Visibility = Visibility.Visible;
            userDataGrid.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Qidiruvda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var addWindow = new CustomerAddWindow(services);
            addWindow.ShowDialog();

            if (addWindow.IsCreated)
            {
                LoadCustomersAsync();
                CustomerAdded?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yangi mijoz oynasini ochishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is CustomerResultDto customer)
            {
                var editWindow = new CustomerEditWindow(services, customer.Id);
                editWindow.ShowDialog();

                if (editWindow.IsModified)
                {
                    LoadCustomersAsync(); // Yangilangan ma'lumotlarni qayta yuklash
                    CustomerAdded?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Tahrirlashda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is CustomerResultDto customer)
            {
                var result = MessageBox.Show($"Rostan ham {customer.CompanyName} mijozini o‘chirmoqchimisiz?", "O‘chirish tasdiqlash", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await customerService.RemoveAsync(customer.Id);
                    MessageBox.Show("Mijoz muvaffaqiyatli o‘chirildi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomersAsync();
                    CustomerAdded?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"O‘chirishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            MessageBox.Show("Mijozlar ro‘yxati Excelga eksport qilinadi (hozircha taxminiy funksiya).", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Excelga eksport qilishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}