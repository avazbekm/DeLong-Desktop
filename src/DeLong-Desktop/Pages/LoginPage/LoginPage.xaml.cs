using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace DeLong_Desktop.Pages.LoginPage;

/// <summary>
/// Interaction logic for LoginPage.xaml
/// </summary>
public partial class LoginPage : Page
{
    private readonly IServiceProvider _serviceProvider;
    private string _currentLanguage = "en";

    public LoginPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        SetLanguage(_currentLanguage);
    }
    private void SetLanguage(string language)
    {
        DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(language);
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

    private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbLanguage.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedLanguage = selectedItem.Tag?.ToString();

            if (!string.IsNullOrEmpty(selectedLanguage) && _currentLanguage != selectedLanguage)
            {
                _currentLanguage = selectedLanguage;
                SetLanguage(_currentLanguage);
                UpdateLanguage(); // Interfeys matnlarini yangilash
            }
        }
    }
    private void UpdateLanguage()
    {
        btnLogin.Content = DeLong_Desktop.Resources.Resource.Login;
        btnName.Text = DeLong_Desktop.Resources.Resource.Delong;
        HintAssist.SetHint(txtUsername, DeLong_Desktop.Resources.Resource.Foydalanuvchi_nomi);
        HintAssist.SetHint(txtPassword, DeLong_Desktop.Resources.Resource.Parol);
    }
}
