using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.Employees;

/// <summary>
/// Interaction logic for EditEmployeeWindow.xaml
/// </summary>
public partial class EditEmployeeWindow : Window
{
    private readonly IUserService _userService;
    private readonly IBranchService _branchService;
    private readonly IServiceProvider _services;
    private readonly UserResultDto _employee;

    public EditEmployeeWindow(IServiceProvider services, UserResultDto employee)
    {
        InitializeComponent();
        _services = services;
        _userService = services.GetRequiredService<IUserService>();
        _branchService = services.GetRequiredService<IBranchService>();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        InitializeComboBoxesAsync();
        FillEmployeeData();
    }

    // ComboBox larni to‘ldirish
    private async void InitializeComboBoxesAsync()
    {
        try
        {
            // Jins
            cmbGender.ItemsSource = new[] { Gender.Erkak, Gender.Ayol };

            // Rol
            cmbRole.ItemsSource = new[] { Role.Agent, Role.Omborchi, Role.Sotuvchi, Role.Boshqaruvchi, Role.Admin };

            // Filiallar
            var branches = await _branchService.RetrieveAllAsync();
            if (!branches.Any())
            {
                MessageBox.Show("Filiallar ro‘yxati bo‘sh. Administrator bilan bog‘laning.", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            cmbBranchId.ItemsSource = branches;
            cmbBranchId.DisplayMemberPath = "BranchName";
            cmbBranchId.SelectedValuePath = "Id";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ComboBox larni yuklashda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Xodim ma’lumotlarini to‘ldirish
    private void FillEmployeeData()
    {
        try
        {
            txtLastName.Text = _employee.LastName.ToUpper();
            txtFirstName.Text = _employee.FirstName.ToUpper();
            txtPatronymic.Text = _employee.Patronomyc.ToUpper();
            txtPassportSeries.Text = _employee.SeriaPasport;
            dpDateOfBirth.SelectedDate = _employee.DateOfBirth?.DateTime;
            dpDateOfIssue.SelectedDate = _employee.DateOfIssue?.DateTime;
            dpDateOfExpiry.SelectedDate = _employee.DateOfExpiry?.DateTime;
            cmbGender.SelectedItem = _employee.Gender;
            txtPhone.Text = _employee.Phone;
            txtAddress.Text = _employee.Address.ToUpper();
            txtJshshir.Text = _employee.JSHSHIR;
            cmbRole.SelectedItem = _employee.Role;
            cmbBranchId.SelectedValue = _employee.BranchId;
            txtUsername.Text = _employee.Username;
            // Parol to‘ldirilmaydi, foydalanuvchi yangi parol kiritishi mumkin
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ma’lumotlarni to‘ldirishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Saqlash tugmasi
    private async void btnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Majburiy maydonlarni tekshirish
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Familiyani kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Ismni kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFirstName.Focus();
                return;
            }

            if (cmbGender.SelectedItem == null)
            {
                MessageBox.Show("Jinsni tanlang!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbGender.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Telefon raqamini kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return;
            }

            if (!ValidationHelper.IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Telefon raqami noto‘g‘ri formatda! (+998 95 5701010 kabi)", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Loginni kiriting!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Rolni tanlang!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbRole.Focus();
                return;
            }

            if (cmbBranchId.SelectedItem == null)
            {
                MessageBox.Show("Filialni tanlang!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbBranchId.Focus();
                return;
            }

            // JSHSHIR dublikatini tekshirish (agar kiritilgan bo‘lsa va o‘zgargan bo‘lsa)
            if (!string.IsNullOrWhiteSpace(txtJshshir.Text) && txtJshshir.Text != _employee.JSHSHIR)
            {
                var existingUser = await _userService.RetrieveByJSHSHIRAsync(txtJshshir.Text);
                if (existingUser != null && existingUser.Id != _employee.Id)
                {
                    MessageBox.Show($"Bu JSHSHIR ({txtJshshir.Text}) bilan xodim allaqachon mavjud!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtJshshir.Focus();
                    return;
                }
            }

            // Telefon raqami dublikatini tekshirish
            if (txtPhone.Text != _employee.Phone)
            {
                var users = await _userService.RetrieveAllAsync();
                if (users.Any(u => u.Phone == txtPhone.Text && u.Id != _employee.Id))
                {
                    MessageBox.Show("Bu telefon raqami allaqachon mavjud!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPhone.Focus();
                    return;
                }
            }

            // Username dublikatini tekshirish
            if (txtUsername.Text != _employee.Username)
            {
                var users = await _userService.RetrieveAllAsync();
                if (users.Any(u => u.Username == txtUsername.Text && u.Id != _employee.Id))
                {
                    MessageBox.Show("Bu login allaqachon mavjud!", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
                    return;
                }
            }

            // UserUpdateDto yaratish
            var userUpdateDto = new UserUpdateDto
            {
                Id = _employee.Id,
                LastName = txtLastName.Text.ToLower().Trim(),
                FirstName = txtFirstName.Text.ToLower().Trim(),
                Patronomyc = string.IsNullOrWhiteSpace(txtPatronymic.Text.ToLower()) ? null : txtPatronymic.Text.ToLower().Trim(),
                SeriaPasport = string.IsNullOrWhiteSpace(txtPassportSeries.Text) ? null : txtPassportSeries.Text.Trim(),
                DateOfBirth = dpDateOfBirth.SelectedDate.HasValue ? new DateTimeOffset(dpDateOfBirth.SelectedDate.Value) : null,
                DateOfIssue = dpDateOfIssue.SelectedDate.HasValue ? new DateTimeOffset(dpDateOfIssue.SelectedDate.Value) : null,
                DateOfExpiry = dpDateOfExpiry.SelectedDate.HasValue ? new DateTimeOffset(dpDateOfExpiry.SelectedDate.Value) : null,
                Gender = (Gender)cmbGender.SelectedItem,
                Phone = txtPhone.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(txtAddress.Text.ToLower()) ? null : txtAddress.Text.ToLower().Trim(),
                JSHSHIR = string.IsNullOrWhiteSpace(txtJshshir.Text) ? null : txtJshshir.Text.Trim(),
                Role = (Role)cmbRole.SelectedItem,
                BranchId = (long)cmbBranchId.SelectedValue,
                Username = txtUsername.Text.Trim(),
                Password = string.IsNullOrWhiteSpace(txtPassword.Password) ? null : txtPassword.Password // Yangi parol kiritilmasa null
            };

            // Serverga yuborish
            var result = await _userService.ModifyAsync(userUpdateDto);
            if (result)
            {
                MessageBox.Show("Xodim muvaffaqiyatli yangilandi!", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Xodim yangilashda xatolik yuz berdi!", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Bekor qilish tugmasi
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Telefon raqamlarini formatlash
    private void FormatPhoneNumber(TextBox textBox)
    {
        if (textBox == null) return;

        string text = textBox.Text?.Trim() ?? string.Empty;
        string digits = Regex.Replace(text, @"[^\d]", ""); // Faqat raqamlarni olish

        // Hodisani vaqtincha o‘chirish
        textBox.TextChanged -= txtPhone_TextChanged;

        try
        {
            // Agar bo‘sh bo‘lsa yoki faqat +998 kiritilgan bo‘lsa
            if (digits.Length <= 3)
            {
                textBox.Text = "+998 ";
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }

            // Raqamlarni formatlash
            string formatted = "+998 ";
            int remainingDigits = digits.Length - 3; // +998 dan keyingi raqamlar

            if (remainingDigits > 0)
            {
                // 2 raqamli kod
                formatted += digits.Substring(3, Math.Min(2, remainingDigits));
                if (remainingDigits > 2)
                {
                    formatted += " ";
                    // Qolgan 7 raqam
                    formatted += digits.Substring(5, Math.Min(7, remainingDigits - 2));
                }
            }

            textBox.Text = formatted.Trim();
            textBox.SelectionStart = textBox.Text.Length;
        }
        finally
        {
            // Hodisani qayta ulash
            textBox.TextChanged += txtPhone_TextChanged;
        }
    }

    // Telefon raqami uchun TextChanged
    private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        FormatPhoneNumber(sender as TextBox);
        ValidationHelper.ValidatePhone(sender as TextBox);
    }

    // Telefon raqamiga faqat raqam kiritish
    private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"[\d]");
    }

    // JSHSHIR uchun validatsiya
    private void txtJshshir_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
    }

    private void txtJshshir_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"[\d]");
    }
}