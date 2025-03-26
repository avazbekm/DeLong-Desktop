using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.LoginPage;

/// <summary>
/// Interaction logic for LoginPage.xaml
/// </summary>
public partial class LoginPage : Page
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _authService = _serviceProvider.GetRequiredService<IAuthService>();
        txtUsername.Focus();
    }

    private async void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text;
        string password = txtPassword.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Foydalanuvchi nomi va parolni kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            string token = await _authService.LoginAsync(username, password);
            if (token != null)
            {
                App.Current.Properties["Token"] = token; // Tokenni saqlash
                var mainWindow = new MainWindow(_serviceProvider); // Agar DI kerak bo‘lsa, qo‘shiladi
                mainWindow.Show();
                Window.GetWindow(this).Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kirishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
