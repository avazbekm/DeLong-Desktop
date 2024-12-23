using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Customers;

/// <summary>
/// Interaction logic for CustomerAddWindow.xaml
/// </summary>
public partial class CustomerAddWindow : Window
{
    private readonly IUserService userService;
    private readonly IServiceProvider services;

    public CustomerAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.userService = services.GetRequiredService<IUserService>();
    }

    private void rbtnYurdik_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Visible;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Hidden;
    }

    private void rbtnYaTT_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Visible;
    }

    private void rbtnJismoniy_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Visible;
        spYattCutomer.Visibility = Visibility.Hidden;
    }
    string gender = "";
    private void btnJisAdd_Click(object sender, RoutedEventArgs e)
    {
        UserCreationDto userCreationDto = new UserCreationDto();

        userCreationDto.LastName = txtFamiliya.Text;
        userCreationDto.FirstName = txtIsmi.Text;
        userCreationDto.Patronomyc = txtSharifi.Text;
        userCreationDto.SeriaPasport = txtPasportSeria.Text;
        userCreationDto.Address = txtJisAdres.Text;
        userCreationDto.Phone = txtJisTelefon.Text;
        userCreationDto.TelegramPhone = txtJisTelegramRaqam.Text;
        userCreationDto.JSHSHIR = long.Parse(txtJisJSHSHIR.Text);
        userCreationDto.DateOfBirth =dateOfBirthPicker.SelectedDate.Value.ToUniversalTime();
        userCreationDto.DateOfIssue = dateOfIssuePicker.SelectedDate.Value.ToUniversalTime();
        userCreationDto.DateOfExpiry = dateOfExpiryPicker.SelectedDate.Value.ToUniversalTime();
        if (gender.Equals("Erkak"))
            userCreationDto.Gender = (Gender)0;
        else if (gender.Equals("Ayol"))
            userCreationDto.Gender = (Gender)1;


        if (userCreationDto.FirstName.Equals("") ||
         userCreationDto.LastName.Equals("") ||
         userCreationDto.Phone.Equals(""))
            MessageBox.Show("Malumotni to'liq kiriting!");
        else
        {
            var result = this.userService.AddAsync(userCreationDto);

            if (!result.IsCompletedSuccessfully)
            {
                MessageBox.Show($" Saqlandi.");
            }
            else
                MessageBox.Show($"{"Saqlashda xatolik"}");
        }
        

    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton selectedRadioButton = sender as RadioButton;
        if (selectedRadioButton != null)
        {
            gender = selectedRadioButton.Content.ToString(); // Tanlangan matn: "Erkak" yoki "Ayol"
        }
        else
        {
            MessageBox.Show("Jinsini tanglang iltimos.");
            return;
        }

    }
}
