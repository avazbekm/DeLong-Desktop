using System.Windows;
using DeLong_Desktop.ApiService.Services;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceCollection services = new ServiceCollection();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICustomerService, CustomerService>();
        //services.AddScoped<ICardService, CardService>();
        //services.AddScoped<IBlockService, BlockService>();
        ////services.AddScoped<IAssetService, AssetService>();



        //// Repositories
        //services.AddScoped<IRepository<User>, Repository<User>>();
        //services.AddScoped<IRepository<Company>, Repository<Company>>();
        //services.AddScoped<IRepository<Card>, Repository<Card>>();
        //services.AddScoped<IRepository<BlockDate>, Repository<BlockDate>>();
        ////services.AddScoped<IRepository<Asset>, Repository<Asset>>();


        // Add AppDbContext to the service collection
        // services.AddDbContext<AppDbContext>();

        var serviceProvider = services.BuildServiceProvider();



        new MainWindow(serviceProvider).Show();
    }
}