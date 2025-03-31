using System.Data;
using System.Windows;
using ClosedXML.Excel;
using System.Windows.Data;
using System.Windows.Controls;
using DeLong_Desktop.Windows.Customers;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Customers;

/// <summary>
/// Interaction logic for CustomersPage.xaml
/// </summary>
public partial class CustomersPage : Page
{
    private readonly IUserService userService;
    private readonly ICustomerService customerService;
    
    private readonly IServiceProvider services;
    public bool IsCreated { get; set; } = false;
    public CustomersPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        userService = services.GetRequiredService<IUserService>();
        customerService = services.GetRequiredService<ICustomerService>();

        Loading();
    }

    private async void Loading()
    {
        //datagridga malumotlar to'plash ushun
        List<Item> items = new List<Item>();
        
        // mijozlarni databasadan chaqirvodik
        var existCustomers = await customerService.RetrieveAllAsync();
        
        // jami mavjud jismoniy shaxslarni chaqirib oldik
        var existUsers = await userService.RetrieveAllAsync();
        
        if (existCustomers is not null)
        {
            foreach (var custom in existCustomers)
            {
                var existUser = existUsers.FirstOrDefault(x => x.Id.Equals(custom.UserId));
                items.Add(new Item()
                {
                    Id = custom.Id,
                    FirmaName = custom.Name.ToUpper(),
                    Name = $"{existUser.LastName.ToUpper()} {existUser.FirstName.ToUpper()} {existUser.Patronomyc.ToUpper()}",
                    Phone = custom.Phone,
                    TelegramPhone = existUser.TelegramPhone,
                    JSHSHIR = existUser.JSHSHIR,
                    Adress = custom.YurAddress.ToUpper(),
                    YurJshshir = custom.JSHSHIR.ToUpper(),
                });
            }
        }
        if (existUsers is not null)
        {
            foreach (var user in existUsers)
            {
                items.Add(new Item()
                {
                    FirmaName = "-",
                    Name = $"{user.LastName.ToUpper()} {user.FirstName.ToUpper()}",
                    Phone = user.Phone,
                    TelegramPhone = user.TelegramPhone,
                    JSHSHIR = user.JSHSHIR,
                    Adress = user.Address.ToUpper(),
                    UserId = user.Id
                });
            }
        }
        userDataGrid.ItemsSource = items; ;
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        CustomerAddWindow customerAddWindow = new CustomerAddWindow(services);
        customerAddWindow.ShowDialog();
        if(customerAddWindow.IsCreated) 
            Loading();
    }

    private async void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<Item> items = new List<Item>();

        // firmalarning rahbarlarini id yig'ib olinyapti jismoniy shaxslarni datagridga qo'shayotganda 2 martadan chiqarmaslik uchun
        List<long> ids = new List<long>();

        // mijozlarni databasadan chaqirvodik
        var existCustomers = await customerService.RetrieveAllAsync();

        // jami mavjud jismoniy shaxslarni chaqirib oldik
        var existUsers = await userService.RetrieveAllAsync();

        userDataGrid.ItemsSource = string.Empty;
        if (existCustomers is not null)
        {
            foreach (var custom in existCustomers)
            {
                if (custom.Name.Contains(txtSearch.Text.ToLower())||
                    custom.Phone.Contains(txtSearch.Text))
                {
                    var existUser = existUsers.FirstOrDefault(x => x.Id.Equals(custom.UserId));
                    items.Add(new Item()
                    {
                        Id = custom.Id,
                        FirmaName = custom.Name.ToUpper(),
                        Name = $"{existUser.LastName.ToUpper()} {existUser.FirstName.ToUpper()} {existUser.Patronomyc.ToUpper()}",
                        Phone = custom.Phone,
                        TelegramPhone = existUser.TelegramPhone,
                        JSHSHIR = existUser.JSHSHIR,
                        Adress = custom.YurAddress.ToUpper(),
                        YurJshshir = custom.JSHSHIR.ToUpper(),
                    });
                    ids.Add(custom.UserId);
                }
            }
        }
        if (existUsers is not null)
        {
            foreach (var user in existUsers)
            {
                //if (!ids.Contains(user.Id))
                //{
                    if (user.LastName.Contains(txtSearch.Text.ToLower()) ||
                        user.FirstName.Contains(txtSearch.Text.ToLower()) ||
                        user.JSHSHIR.Contains(txtSearch.Text) ||
                        user.Phone.Contains(txtSearch.Text) ||
                        user.TelegramPhone.Contains(txtSearch.Text))

                        items.Add(new Item()
                        {
                            FirmaName = "-",
                            Name = $"{user.LastName.ToUpper()} {user.FirstName.ToUpper()}",
                            Phone = user.Phone,
                            TelegramPhone = user.TelegramPhone,
                            JSHSHIR = user.JSHSHIR,
                            Adress = user.Address.ToUpper(),
                            UserId = user.Id
                        });
                //}
            }
        }
        userDataGrid.ItemsSource = items; ;

    }

    private void btnExcel_Click(object sender, RoutedEventArgs e)
    {
        // DataGrid ma'lumotlarini DataTable ga o'girish
        DataTable dataTable = new DataTable();

        foreach (var column in userDataGrid.Columns)
        {
            dataTable.Columns.Add(column.Header.ToString());
        }

        foreach (var item in userDataGrid.Items)
        {
            DataRow row = dataTable.NewRow();
            foreach (var column in userDataGrid.Columns)
            {
                if (column is DataGridTextColumn textColumn)
                {
                    Binding binding = textColumn.Binding as Binding;
                    string propertyName = binding?.Path.Path;

                    if (propertyName != null && item != null)
                    {
                        var value = item.GetType().GetProperty(propertyName)?.GetValue(item, null);
                        row[column.Header.ToString()] = value ?? "";
                    }
                }
            }
            dataTable.Rows.Add(row);
        }

        // Excelga yozish
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Exported Data");
            worksheet.Cell(1, 1).InsertTable(dataTable);

            // Fayl saqlash dialog oynasi
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Fayl (*.xlsx)|*.xlsx",
                FileName = "Hamkorlar ro'yxati.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Excelga o'tkazildi!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    private async void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        if (userDataGrid.SelectedItem is Item selectedUser)
        {
            if (!selectedUser.FirmaName.Equals("-"))
            {
                // Tasdiqlash uchun MessageBox ko'rsatish
                var result = MessageBox.Show(
                    $"{selectedUser.FirmaName} o'chirmoqchimisiz?", // Xabar matni
                    "Tasdiqlash",                                  // Oyna sarlavhasi
                    MessageBoxButton.YesNo,                       // HA/YO'Q tugmalari
                    MessageBoxImage.Question);                    // Savol belgisi bilan tasvir

                // Foydalanuvchi "Yes" tugmasini bossagina o'chirish
                if (result == MessageBoxResult.Yes)
                {
                    var existCustomer = await this.customerService.RemoveAsync(selectedUser.Id);
                    if (existCustomer.Equals(true))
                        // O'chirish kodi bu yerga qo'shiladi
                        MessageBox.Show($"{selectedUser.FirmaName} o'chirildi!", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else 
            {
                // Tasdiqlash uchun MessageBox ko'rsatish
                var result = MessageBox.Show(
                    $"{selectedUser.Name} o'chirmoqchimisiz?", // Xabar matni
                    "Tasdiqlash",                                  // Oyna sarlavhasi
                    MessageBoxButton.YesNo,                       // HA/YO'Q tugmalari
                    MessageBoxImage.Question);                    // Savol belgisi bilan tasvir

                // Foydalanuvchi "Yes" tugmasini bossagina o'chirish
                if (result == MessageBoxResult.Yes)
                {
                    var existUser = await this.userService.RemoveAsync(selectedUser.UserId);
                    if (existUser.Equals(true))
                        // O'chirish kodi bu yerga qo'shiladi
                        MessageBox.Show($"{selectedUser.Name} o'chirildi!", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

    private async void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        CustomerEditWindow customerEditWindow = new CustomerEditWindow(services);
        if (userDataGrid.SelectedItem is Item selectedUser)
        {
            CustomerInfo.CustomerId = selectedUser.Id;
            CustomerInfo.UserJshshir = selectedUser.JSHSHIR;
            CustomerInfo.YurJshshir = selectedUser.YurJshshir;
        }

        if (CustomerInfo.CustomerId > 0 && CustomerInfo.YurJshshir.Equals(""))
        {
            customerEditWindow.spYurCutomer.Visibility = Visibility.Visible;
            customerEditWindow.spYattCutomer.Visibility = Visibility.Hidden;
            customerEditWindow.spJisCutomer.Visibility = Visibility.Hidden;
            customerEditWindow.spJisNew.Visibility = Visibility.Hidden;
            customerEditWindow.spQaytish.Visibility = Visibility.Hidden;
            customerEditWindow.spEmployee.Visibility = Visibility.Hidden;

            var existCustomer = await this.customerService.RetrieveByIdAsync(CustomerInfo.CustomerId);
            CustomerInfo.UserId = existCustomer.UserId;
            customerEditWindow.txtYurINN.Text = existCustomer.INN.ToString();
        }
        else if (CustomerInfo.CustomerId > 0 && !CustomerInfo.YurJshshir.Equals(""))
        {
            customerEditWindow.spYurCutomer.Visibility = Visibility.Hidden;
            customerEditWindow.spYattCutomer.Visibility = Visibility.Visible;
            customerEditWindow.spJisCutomer.Visibility = Visibility.Hidden;
            customerEditWindow.spJisNew.Visibility = Visibility.Hidden;
            customerEditWindow.spQaytish.Visibility = Visibility.Hidden;
            customerEditWindow.spEmployee.Visibility = Visibility.Hidden;

            var existYaTT = await this.customerService.RetrieveByJshshirAsync(CustomerInfo.YurJshshir);
            customerEditWindow.txtYattJSHSHIR.Text = existYaTT.JSHSHIR;
            CustomerInfo.UserId = existYaTT.UserId;
        }
        else if (CustomerInfo.CustomerId == 0)
        {
            var existUser = await this.userService.RetrieveByJSHSHIRAsync(CustomerInfo.UserJshshir);
            if (existUser.Role.Equals(Role.Mijoz))
            {
                customerEditWindow.spYurCutomer.Visibility = Visibility.Hidden;
                customerEditWindow.spYattCutomer.Visibility = Visibility.Hidden;
                customerEditWindow.spJisCutomer.Visibility = Visibility.Visible;
                customerEditWindow.spJisNew.Visibility = Visibility.Hidden;
                customerEditWindow.spQaytish.Visibility = Visibility.Hidden;
                customerEditWindow.spEmployee.Visibility = Visibility.Hidden;

                CustomerInfo.UserId = existUser.Id;
                customerEditWindow.txtJisJSHSHIR.Text = existUser.JSHSHIR;
            }
            else
            {
                customerEditWindow.spYurCutomer.Visibility = Visibility.Hidden;
                customerEditWindow.spYattCutomer.Visibility = Visibility.Hidden;
                customerEditWindow.spJisCutomer.Visibility = Visibility.Hidden;
                customerEditWindow.spJisNew.Visibility = Visibility.Hidden;
                customerEditWindow.spQaytish.Visibility = Visibility.Hidden;
                customerEditWindow.spEmployee.Visibility = Visibility.Visible;

                CustomerInfo.UserId = existUser.Id;
                customerEditWindow.txtEmployeeJSHSHIR.Text = existUser.JSHSHIR;
                // lavozimlarni to'ldirib olamiz
                customerEditWindow.cmbRoles.ItemsSource = Enum.GetValues(typeof(Role));
            }
        }

        customerEditWindow.ShowDialog();
        if (customerEditWindow.IsModified) // Agar o'zgarish bo'lsa
        {
            Loading(); // Ma'lumotlarni yangilaymiz
        }
    }
}
