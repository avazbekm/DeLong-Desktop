using System.Windows;
using DeLong.Service.Services;
using DeLong_Desktop.Windows.Login;
using DeLong_Desktop.ApiService.Services;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceCollection services = new ServiceCollection();
        var apiBaseUrl = new Uri("http://localhost:5208/");

        // AuthDelegatingHandler qo‘shish
        services.AddTransient<AuthDelegatingHandler>();

        // Servislar
        services.AddHttpClient<IUserService, UserService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICustomerService, CustomerService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICategoryService, CategoryService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IPriceService, PriceService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IProductService, ProductService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IKursDollarService, KursDollarService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IEmployeeService, EmployeeService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ISaleService, SaleService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ISaleItemService, SaleItemService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IDebtService, DebtService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IDebtPaymentService, DebtPaymentService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IDiscountService, DiscountService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IPaymentService, PaymentService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IReturnProductService, ReturnProductService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ITransactionService, TransactionService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ITransactionItemService, TransactionItemService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICashRegisterService, CashRegisterService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICashTransferService, CashTransferService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICashWarehouseService, CashWarehouseService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IAuthService, AuthService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<IBranchService, BranchService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ISupplierService, SupplierService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICreditorDebtService, CreditorDebtService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ICreditorDebtPaymentService, CreditorDebtPaymentService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddHttpClient<ITransactionProcessingService, TransactionProcessingService>()
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthDelegatingHandler>();

        var serviceProvider = services.BuildServiceProvider();
        new LoginWindow(serviceProvider).Show();
    }
}