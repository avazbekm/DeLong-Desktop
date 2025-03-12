using System.Windows;
using System.Windows.Controls;

namespace DeLong_Desktop.Pages.LoginPage;

/// <summary>
/// Interaction logic for LoginPage.xaml
/// </summary>
public partial class LoginPage : Page
{
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text;
        string password = txtPassword.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Foydalanuvchi nomi va parolni kiriting!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (username == "admin" && password == "1234")
        {
            try
            {
                var loginWindow = Window.GetWindow(this);
                var mainWindow = new MainWindow(_serviceProvider);
                mainWindow.Show();
                loginWindow.Close(); // MainWindow ochilgandan keyin yopamiz
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MainWindow ochishda xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Noto‘g‘ri foydalanuvchi nomi yoki parol!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
