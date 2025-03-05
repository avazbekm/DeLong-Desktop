using System.Windows;
using DeLong_Desktop.Pages.LoginPage;

namespace DeLong_Desktop.Windows.Login;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public LoginWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        LoginFrame.Navigate(new LoginPage(_serviceProvider));
    }
}
