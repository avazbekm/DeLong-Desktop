using System.Windows.Controls;
using DeLong_Desktop.Windows.Customers;
using DeLong_Desktop.ApiService.Interfaces;
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
        
        // firmalarning rahbarlarini id yig'ib olinyapti jismoniy shaxslarni datagridga qo'shayotganda 2 martadan chiqarmaslik uchun
        List<long> ids = new List<long>();

        // mijozlarni databasadan chaqirvodik
        //var existCustomers = await customerService.RetrieveAllAsync();
        
        // jami mavjud jismoniy shaxslarni chaqirib oldik
        var existUsers = await userService.RetrieveAllAsync();
        
        int Tartib = 1;  // datagridga tartib raqam uchun
        //if (existCustomers is not null)
        //{
        //    foreach (var custom in existCustomers)
        //    {
        //        var existUser = existUsers.FirstOrDefault(x => x.Id.Equals(custom.UserId));
        //        items.Add(new Item()
        //        {
        //            Id = Tartib,
        //            FirmaName = custom.Name,
        //            Name = $"{existUser.LastName} {existUser.FirstName} {existUser.Patronomyc}",
        //            Phone = existUser.Phone,
        //            TelegramPhone = existUser.TelegramPhone,
        //            JSHSHIR = existUser.JSHSHIR,
        //            Adress = custom.YurAddress
        //        });
        //        Tartib++;
        //        ids.Add(custom.UserId);
        //    }
        //}
        if (existUsers is not null)
        {
            foreach (var user in existUsers)
            {
                if (!ids.Contains(user.Id))
                {
                    items.Add(new Item()
                    {
                        Id = Tartib,
                        FirmaName = "-",
                        Name = $"{user.LastName} {user.FirstName}",
                        Phone = user.Phone,
                        TelegramPhone = user.TelegramPhone,
                        JSHSHIR = user.JSHSHIR,
                        Adress = user.Address
                    });
                    Tartib++;
                }
            }
        }
        userDataGrid.ItemsSource = items; ;
    }

    private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CustomerAddWindow customerAddWindow = new CustomerAddWindow(services);
        customerAddWindow.ShowDialog();
    }
}
