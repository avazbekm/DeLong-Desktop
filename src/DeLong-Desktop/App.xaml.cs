using System.Windows;
using DeLong.Service.Services;
using DeLong_Desktop.Windows.Login;
using DeLong_Desktop.ApiService.Services;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceCollection services = new ServiceCollection();

        // **Barcha servislar uchun umumiy API manzili**
        var apiBaseUrl = new Uri("http://13.61.150.163/");

        // **Servislarni qo‘shish (Clean Code)**
        services.AddHttpClient<IUserService, UserService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ICustomerService, CustomerService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);


        services.AddHttpClient<ICategoryService, CategoryService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IPriceService, PriceService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IProductService, ProductService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IWarehouseService, WarehouseService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IKursDollarService, KursDollarService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IEmployeeService, EmployeeService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ISaleService, SaleService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ISaleItemService, SaleItemService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IDebtService, DebtService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IDebtPaymentService, DebtPaymentService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IDiscountService, DiscountService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IPaymentService, PaymentService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<IReturnProductService, ReturnProductService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ITransactionService, TransactionService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ITransactionItemService, TransactionItemService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ICashRegisterService, CashRegisterService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ICashTransferService, CashTransferService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);

        services.AddHttpClient<ICashWarehouseService, CashWarehouseService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl);


        var serviceProvider = services.BuildServiceProvider();
        new LoginWindow(serviceProvider).Show();
    }
}
