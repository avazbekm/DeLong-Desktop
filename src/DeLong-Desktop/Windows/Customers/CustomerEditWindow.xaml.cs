using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Customers;
using DeLong_Desktop.ApiService.DTOs.Employees;

namespace DeLong_Desktop.Windows.Customers;

/// <summary>
/// Interaction logic for CustomerEditWindow.xaml
/// </summary>
public partial class CustomerEditWindow : Window
{
    private readonly IUserService userService;
    private readonly ICustomerService customerService;
    private readonly IEmployeeService employeeService;
    private readonly IServiceProvider services;
    public bool IsModified { get; set; } = false; // `IsCreated` o'rniga `IsModified` ishlatildi
    string gender = "";
    string userJshshir = "";

    public CustomerEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.userService = services.GetRequiredService<IUserService>();
        this.customerService = services.GetRequiredService<ICustomerService>();
        this.employeeService = services.GetRequiredService<IEmployeeService>();
    }

    // radio buttonlar
    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton selectedRadioButton = sender as RadioButton;
        if (selectedRadioButton != null)
        {
            gender = selectedRadioButton.Content.ToString(); // Tanlangan matn: "Erkak" yoki "Ayol"
        }
        else
        {
            MessageBox.Show("Jinsini tanlang iltimos.");
            return;
        }
    }

    // qo'shimcha funksiya
    // xisob raqamni - larni olib tashlaydigan function
    public string RemoveDashes(string input)
        => new string(input.Where(char.IsDigit).ToArray());

    //TextBoxlar
    private async void txtJisJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);

        if (txtJisJSHSHIR.Text.Length.Equals(14))
        {
            var existUser = await this.userService.RetrieveByJSHSHIRAsync(txtJisJSHSHIR.Text);
            if (existUser is not null)
            {
                txtFamiliya.Text = existUser.LastName.ToUpper();
                txtIsmi.Text = existUser.FirstName.ToUpper();
                txtSharifi.Text = existUser.Patronomyc.ToUpper();
                txtPasportSeria.Text = existUser.SeriaPasport;
                txtJisAdres.Text = existUser.Address.ToUpper();
                txtJisTelefon.Text = existUser.Phone;
                txtJisTelegramRaqam.Text = existUser.TelegramPhone;
            }
        }
    }

    private void txtJisTelefon_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private void txtJisTelegramRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private void txtPasportSeria_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePasportInformation(textBox);
    }

    private async void txtYurINN_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);

        if (txtYurINN.Text.Length == 9)
        {
            var inn = int.Parse(txtYurINN.Text);
            var existCustomer = await this.customerService.RetrieveByInnAsync(inn);
            if (existCustomer is not null)
            {
                txtYurNomi.Text = existCustomer.Name.ToUpper();
                txtYurPhone.Text = existCustomer.Phone;
                txtYurMFO.Text = existCustomer.MFO;
                txtYurXisobRaqam.Text = existCustomer.BankAccount;
                txtYurBank.Text = existCustomer.BankName.ToUpper();
                txtYurOKONX.Text = existCustomer.OKONX;
                txtYurFirmaAdres.Text = existCustomer.YurAddress.ToUpper();
            }
        }
    }

    private void txtYurMFO_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);
    }

    private void txtYurOKONX_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);
    }

    private void txtYurXisobRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateAccountNumber(textBox);
    }

    private async void txtYattJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);

        if (txtYattJSHSHIR.Text.Length.Equals(14))
        {
            var existYaTT = await this.customerService.RetrieveByJshshirAsync(txtYattJSHSHIR.Text);
            if (existYaTT is not null)
            {
                txtYattNomi.Text = existYaTT.Name.ToUpper();
                txtYattMFO.Text = existYaTT.MFO;
                txtYattXisobRaqam.Text = existYaTT.BankAccount;
                txtYattBank.Text = existYaTT.BankName.ToUpper();
                txtYattFirmaAdres.Text = existYaTT.YurAddress.ToUpper();
                txtYattTelefon.Text = existYaTT.Phone;
            }
        }
    }

    private void txtYurPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private void txtYattXisobRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateAccountNumber(textBox);
    }

    private void txtYattMFO_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);
    }

    private void txtYattTelefon_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private void txtSearchJ_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidateOnlyNumberInput(textBox);
    }

    // buttonlar
    private void btnRahbar_Click(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Hidden;
        spQaytish.Visibility = Visibility.Visible;
        spJisNew.Visibility = Visibility.Visible;
    }

    private void rbtnYurdik_Checked(object sender, RoutedEventArgs e)
    {
    }

    private void btnQaytish_Click(object sender, RoutedEventArgs e)
    {
        if (CustomerInfo.CustomerId > 0 && CustomerInfo.YurJshshir.Equals(""))
        {
            spYurCutomer.Visibility = Visibility.Visible;
            spYattCutomer.Visibility = Visibility.Hidden;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility = Visibility.Hidden;
        }
        else if (CustomerInfo.CustomerId > 0 && !CustomerInfo.YurJshshir.Equals(""))
        {
            spYurCutomer.Visibility = Visibility.Hidden;
            spYattCutomer.Visibility = Visibility.Visible;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility = Visibility.Hidden;
        }
    }

    private void btnJisAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            UserUpdateDto userUpdateDto = new UserUpdateDto();

            userUpdateDto.Id = CustomerInfo.UserId;
            userUpdateDto.LastName = txtFamiliya.Text.ToLower();
            userUpdateDto.FirstName = txtIsmi.Text.ToLower();
            userUpdateDto.Patronomyc = txtSharifi.Text.ToLower();
            userUpdateDto.SeriaPasport = txtPasportSeria.Text.ToLower();
            userUpdateDto.Address = txtJisAdres.Text.ToLower();
            userUpdateDto.Phone = txtJisTelefon.Text;
            userUpdateDto.TelegramPhone = txtJisTelegramRaqam.Text;
            userUpdateDto.JSHSHIR = txtJisJSHSHIR.Text;
            userJshshir = txtJisJSHSHIR.Text;

            if (!dateOfBirthPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Tug'ulgan sanani kiriting iltimos.");
                return;
            }
            userUpdateDto.DateOfBirth = dateOfBirthPicker.SelectedDate.Value.ToUniversalTime();

            if (!dateOfIssuePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Pasport berilgan sanani kiriting iltimos.");
                return;
            }

            userUpdateDto.DateOfIssue = dateOfIssuePicker.SelectedDate.Value.ToUniversalTime();
            if (!dateOfExpiryPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Pasportni amal qilish sanani kiriting iltimos.");
                return;
            }
            userUpdateDto.DateOfExpiry = dateOfExpiryPicker.SelectedDate.Value.ToUniversalTime();
            if (gender.Equals("Erkak"))
                userUpdateDto.Gender = (Gender)0;
            else if (gender.Equals("Ayol"))
                userUpdateDto.Gender = (Gender)1;

            if (userUpdateDto.FirstName.Equals("") ||
                userUpdateDto.LastName.Equals("") ||
                userUpdateDto.SeriaPasport.Equals("") ||
                userUpdateDto.Address.Equals("") ||
                userUpdateDto.JSHSHIR.Equals("") ||
                userUpdateDto.TelegramPhone.Equals("") ||
                userUpdateDto.Phone.Equals(""))
            {
                MessageBox.Show("Malumotni to'liq kiriting!");
            }
            else
            {
                var result = this.userService.ModifyAsync(userUpdateDto);

                if (!result.IsCompletedSuccessfully)
                {
                    MessageBox.Show("Muvaffaqiyatli saqlandi.");
                    IsModified = true; // O'zgarish bo'lganini belgilaymiz
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Saqlashda xatolik yuz berdi.");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void btnYattAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CustomerUpdateDto customerUpdateDto = new CustomerUpdateDto();

            customerUpdateDto.Id = CustomerInfo.CustomerId;
            customerUpdateDto.Name = txtYattNomi.Text.ToLower();

            if (txtYattJSHSHIR.Text.Length != 14)
            {
                MessageBox.Show("JSHSHIR ni to'liq kiriting iltimos.");
                return;
            }
            customerUpdateDto.JSHSHIR = txtYattJSHSHIR.Text;
            customerUpdateDto.MFO = txtYattMFO.Text;

            if (txtYattXisobRaqam.Text.Length != 24)
            {
                MessageBox.Show("Xisob raqamni to'liq kiriting iltimos.");
                return;
            }
            customerUpdateDto.BankAccount = RemoveDashes(txtYattXisobRaqam.Text);

            customerUpdateDto.BankName = txtYattBank.Text.ToLower();
            customerUpdateDto.YurAddress = txtYattFirmaAdres.Text.ToLower();
            customerUpdateDto.Phone = txtYattTelefon.Text;

            if (!userJshshir.Equals(""))
            {
                var existUser = await this.userService.RetrieveByJSHSHIRAsync(userJshshir);
                customerUpdateDto.UserId = existUser.Id;
            }
            else
            {
                customerUpdateDto.UserId = CustomerInfo.UserId;
            }

            if (customerUpdateDto.Name.Equals("") ||
                customerUpdateDto.Phone.Equals("") ||
                customerUpdateDto.MFO.Equals("") ||
                customerUpdateDto.BankName.Equals("") ||
                customerUpdateDto.YurAddress.Equals(""))
            {
                MessageBox.Show("Ma'lumotni to'liq kiriting!");
            }
            else
            {
                var result = await customerService.ModifyAsync(customerUpdateDto);

                if (result)
                {
                    MessageBox.Show("Muvaffaqiyatli saqlandi.");
                    IsModified = true; // O'zgarish bo'lganini belgilaymiz
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Saqlashda xatolik yuz berdi.");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void btnYaTTRahbar_Click(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Hidden;
        spJisNew.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Visible;
    }

    private async void btnSearch_Click(object sender, RoutedEventArgs e)
    {
        var existUser = await this.userService.RetrieveByJSHSHIRAsync(txtSearchJ.Text);

        if (existUser is not null)
        {
            btnExistClient.Visibility = Visibility.Visible;
            userJshshir = existUser.JSHSHIR;
        }
        else
            MessageBox.Show($"{txtSearchJ.Text} Jshshirli hamkor mavjud emas.");
    }

    private void btnExistClient_Click(object sender, RoutedEventArgs e)
    {
        if (CustomerInfo.CustomerId > 0 && CustomerInfo.YurJshshir.Equals(""))
        {
            spYurCutomer.Visibility = Visibility.Visible;
            spYattCutomer.Visibility = Visibility.Hidden;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility = Visibility.Hidden;
        }
        else if (CustomerInfo.CustomerId > 0 && !CustomerInfo.YurJshshir.Equals(""))
        {
            spYurCutomer.Visibility = Visibility.Hidden;
            spYattCutomer.Visibility = Visibility.Visible;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility = Visibility.Hidden;
        }
    }

    private async void btnYurAdd_Click(object sender, RoutedEventArgs e)
    {
        CustomerUpdateDto customerUpdateDto = new CustomerUpdateDto();

        customerUpdateDto.Id = CustomerInfo.CustomerId;
        customerUpdateDto.Name = txtYurNomi.Text.ToLower();
        if (txtYurINN.Text.Length != 9)
        {
            MessageBox.Show("INN kiriting iltimos.");
            return;
        }
        customerUpdateDto.INN = int.Parse(txtYurINN.Text);
        customerUpdateDto.MFO = txtYurMFO.Text;

        if (txtYurXisobRaqam.Text.Length != 24)
        {
            MessageBox.Show("Xisob raqamni to'liq kiriting iltimos.");
            return;
        }
        customerUpdateDto.Phone = txtYurPhone.Text;
        customerUpdateDto.BankAccount = RemoveDashes(txtYurXisobRaqam.Text);
        customerUpdateDto.BankName = txtYurBank.Text.ToLower();
        customerUpdateDto.OKONX = txtYurOKONX.Text;
        customerUpdateDto.YurAddress = txtYurFirmaAdres.Text.ToLower();

        if (!userJshshir.Equals(""))
        {
            var user = await userService.RetrieveByJSHSHIRAsync(userJshshir);
            customerUpdateDto.UserId = user.Id;
        }
        else
            customerUpdateDto.UserId = CustomerInfo.UserId;

        if (customerUpdateDto.Name.Equals("") ||
           customerUpdateDto.INN.Equals("") ||
           customerUpdateDto.Phone.Equals("") ||
           customerUpdateDto.MFO.Equals("") ||
           customerUpdateDto.BankName.Equals("") ||
           customerUpdateDto.OKONX.Equals("") ||
           customerUpdateDto.YurAddress.Equals(""))
        {
            MessageBox.Show("Ma'lumotni to'liq kiriting!");
        }
        else
        {
            var result = await this.customerService.ModifyAsync(customerUpdateDto);

            if (result)
            {
                MessageBox.Show("Muvaffaqiyatli saqlandi.");
                IsModified = true; // O'zgarish bo'lganini belgilaymiz
                this.Close();
            }
            else
            {
                MessageBox.Show("Saqlashda xatolik yuz berdi.");
            }
        }
    }

    private async void txtEmployeeJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var textBox = sender as TextBox;
            ValidationHelper.ValidateOnlyNumberInput(textBox);

            if (txtEmployeeJSHSHIR.Text.Length.Equals(14))
            {
                var existUser = await this.userService.RetrieveByJSHSHIRAsync(txtEmployeeJSHSHIR.Text);
                if (existUser is not null)
                {
                    txtEmployeLastname.Text = existUser.LastName.ToUpper();
                    txtEmployeFirstname.Text = existUser.FirstName.ToUpper();
                    txtEmployeePatro.Text = existUser.Patronomyc.ToUpper();
                    txtEmployeePasportSeria.Text = existUser.SeriaPasport;
                    txtEmployeeAddress.Text = existUser.Address.ToUpper();
                    txtEmployeeTelefon.Text = existUser.Phone;
                    txtEmployeeTelegramRaqam.Text = existUser.TelegramPhone;
                    cmbRoles.SelectedItem = existUser.Role; // Avtomatik role tanlash

                    var positions = await this.employeeService.RetrieveAllAsync();
                    var position = positions.FirstOrDefault(u => u.UserId.Equals(existUser.Id));
                    if (position is not null)
                    {
                        CustomerInfo.EmployeeId = position.Id;
                        txtLogin.Text = position.Username;
                    }
                    else
                    {
                        MessageBox.Show("Login va parol topilmadi.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void txtEmployeePasportSeria_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePasportInformation(textBox);
    }

    private void txtEmployeeTelefon_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private void txtEmployeeTelegramRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        ValidationHelper.ValidatePhone(textBox);
    }

    private async void btnEmployeeUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            UserUpdateDto userUpdateDto = new UserUpdateDto();

            userUpdateDto.Id = CustomerInfo.UserId;
            userUpdateDto.LastName = txtEmployeLastname.Text.ToLower();
            userUpdateDto.FirstName = txtEmployeFirstname.Text.ToLower();
            userUpdateDto.Patronomyc = txtEmployeePatro.Text.ToLower();
            userUpdateDto.SeriaPasport = txtEmployeePasportSeria.Text.ToLower();
            userUpdateDto.Address = txtEmployeeAddress.Text.ToLower();
            userUpdateDto.Phone = txtEmployeeTelefon.Text;
            userUpdateDto.TelegramPhone = txtEmployeeTelegramRaqam.Text;
            userUpdateDto.JSHSHIR = txtEmployeeJSHSHIR.Text;
            userUpdateDto.Role = (Role)cmbRoles.SelectedItem;

            userJshshir = txtEmployeeJSHSHIR.Text;

            if (!dateOfBirthPickerEmployee.SelectedDate.HasValue)
            {
                MessageBox.Show("Tug'ulgan sanani kiriting iltimos.");
                return;
            }
            userUpdateDto.DateOfBirth = dateOfBirthPickerEmployee.SelectedDate.Value.ToUniversalTime();

            if (!dateOfIssuePickerEmployee.SelectedDate.HasValue)
            {
                MessageBox.Show("Pasport berilgan sanani kiriting iltimos.");
                return;
            }

            userUpdateDto.DateOfIssue = dateOfIssuePickerEmployee.SelectedDate.Value.ToUniversalTime();
            if (!dateOfExpiryPickerEmployee.SelectedDate.HasValue)
            {
                MessageBox.Show("Pasportni amal qilish sanani kiriting iltimos.");
                return;
            }
            userUpdateDto.DateOfExpiry = dateOfExpiryPickerEmployee.SelectedDate.Value.ToUniversalTime();
            if (gender.Equals("Erkak"))
                userUpdateDto.Gender = (Gender)0;
            else if (gender.Equals("Ayol"))
                userUpdateDto.Gender = (Gender)1;

            if (userUpdateDto.FirstName.Equals("") ||
                userUpdateDto.LastName.Equals("") ||
                userUpdateDto.SeriaPasport.Equals("") ||
                userUpdateDto.Address.Equals("") ||
                userUpdateDto.JSHSHIR.Equals("") ||
                userUpdateDto.TelegramPhone.Equals("") ||
                userUpdateDto.Phone.Equals(""))
            {
                MessageBox.Show("Malumotni to'liq kiriting!");
            }
            else
            {
                var result = await this.userService.ModifyAsync(userUpdateDto);

                if (result)
                {
                    EmployeeUpdateDto employeeUpdateDto = new EmployeeUpdateDto();
                    employeeUpdateDto.Id = CustomerInfo.EmployeeId;
                    employeeUpdateDto.UserId = CustomerInfo.UserId;
                    employeeUpdateDto.Username = txtLogin.Text;
                    employeeUpdateDto.Password = txtParol.Text;

                    if (txtLogin.Text.Length == 0 || txtParol.Text.Length == 0)
                    {
                        MessageBox.Show("Login yoki Parolni kiriting");
                        return;
                    }

                    var empoloyeeUpdate = await this.employeeService.ModifyAsync(employeeUpdateDto);
                    if (empoloyeeUpdate != null)
                    {
                        MessageBox.Show("Muvaffaqiyatli o'zgartirildi.");
                        IsModified = true; // O'zgarish bo'lganini belgilaymiz
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login yoki Parolni saqlashda xatolik");
                    }
                }
                else
                {
                    MessageBox.Show("Saqlashda xatolik yuz berdi.");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}