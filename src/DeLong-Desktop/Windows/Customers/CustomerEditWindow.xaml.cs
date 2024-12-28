using System.Text;
using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.Pages.Customers;
using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DeLong_Desktop.ApiService.DTOs.Customers;

namespace DeLong_Desktop.Windows.Customers;

/// <summary>
/// Interaction logic for CustomerEditWindow.xaml
/// </summary>
public partial class CustomerEditWindow : Window
{
    private readonly IUserService userService;
    private readonly ICustomerService customerService;
    private readonly IServiceProvider services;
    public bool IsCreated { get; set; } = false;
    string gender = "";
    string userJshshir = "";
    public CustomerEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.userService = services.GetRequiredService<IUserService>();
        this.customerService = services.GetRequiredService<ICustomerService>();
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
            MessageBox.Show("Jinsini tanglang iltimos.");
            return;
        }

    }

    // qo'shimcha funsiya
    // xisob raqamni - larni olib tashlaydigan function
    public string RemoveDashes(string input)
        => new string(input.Where(char.IsDigit).ToArray());

    //TextBoxlar
    private async void txtJisJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }

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
        if (textBox != null)
        {
            // Faqat raqamlar va bo'sh joylarni qoldirish
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == ' ').ToArray());

            // Agar noto'g'ri belgilar kirgan bo'lsa, matnni yangilash
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni to'g'ri joylash
            }
        }
    }
    private void txtJisTelegramRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null)
        {
            // Faqat raqamlar, bo'sh joy va "+" belgilarini qoldirish
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == ' ' || c == '+').ToArray());

            // Agar noto'g'ri belgilar kirgan bo'lsa, matnni yangilash
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni to'g'ri joylash
            }
        }
    }
    private void txtPasportSeria_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null)
        {
            string inputText = textBox.Text.ToUpper(); // Matnni katta harflarga o‘zgartirish
            string filteredText = string.Empty;

            for (int i = 0; i < inputText.Length; i++)
            {
                if (i < 2) // Birinchi 2 ta belgi uchun faqat katta harflar
                {
                    if (char.IsLetter(inputText[i]) && char.IsUpper(inputText[i]))
                    {
                        filteredText += inputText[i];
                    }
                }
                else if (i < 9) // Keyingi 7 ta belgi uchun faqat raqamlar
                {
                    if (char.IsDigit(inputText[i]))
                    {
                        filteredText += inputText[i];
                    }
                }
            }

            // Maksimal uzunlikni cheklash (2 harf + 7 raqam)
            if (filteredText.Length > 9)
            {
                filteredText = filteredText.Substring(0, 9);
            }

            // Matnni yangilash, agar noto'g'ri kirishlar bo‘lsa
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Ko'rsatkichni saqlash
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni tiklash
            }
        }
    }
    private async void txtYurINN_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }

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

                btnYurAdd.Visibility = Visibility.Hidden;
                MessageBox.Show($"INN:{txtYurINN.Text} mijoz mavjud. Qayta ro'yxatdan o'tkazish shart emas. ");
            }
        }
    }
    private void txtYurMFO_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }
    }
    private void txtYurOKONX_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }
    }
    private void txtYurXisobRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox == null) return;

        // Retrieve only digits from the current text
        string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

        // Format: XXXXX-XXX-X-XXXXXXXX-XXX
        StringBuilder formattedText = new StringBuilder();
        int[] groupSizes = { 5, 3, 1, 8, 3 };
        int currentIndex = 0;

        foreach (int groupSize in groupSizes)
        {
            if (numericText.Length > currentIndex)
            {
                int charsToTake = Math.Min(groupSize, numericText.Length - currentIndex);
                formattedText.Append(numericText.Substring(currentIndex, charsToTake));
                currentIndex += charsToTake;

                // Add a separator unless it's the last group
                if (currentIndex < numericText.Length && formattedText.Length < 20)
                    formattedText.Append('-');
            }
        }

        // Save the original caret position
        int oldCaretIndex = textBox.CaretIndex;

        // Update the text only if it has changed
        if (textBox.Text != formattedText.ToString())
        {
            textBox.Text = formattedText.ToString();

            // Adjust caret position dynamically
            int newCaretIndex = oldCaretIndex;

            // Prevent cursor from being stuck after separators
            if (oldCaretIndex > 0 && oldCaretIndex < formattedText.Length && formattedText[oldCaretIndex - 1] == '-')
            {
                newCaretIndex++;
            }

            textBox.CaretIndex = Math.Min(newCaretIndex, textBox.Text.Length);
        }
    }
    private async void txtYattJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }

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
        if (textBox != null)
        {
            // Faqat raqamlar va bo'sh joylarni qoldirish
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == ' ').ToArray());

            // Agar noto'g'ri belgilar kirgan bo'lsa, matnni yangilash
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni to'g'ri joylash
            }
        }
    }
    private void txtYattXisobRaqam_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox == null) return;

        // Retrieve only digits from the current text
        string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

        // Format: XXXXX-XXX-X-XXXXXXXX-XXX
        StringBuilder formattedText = new StringBuilder();
        int[] groupSizes = { 5, 3, 1, 8, 3 };
        int currentIndex = 0;

        foreach (int groupSize in groupSizes)
        {
            if (numericText.Length > currentIndex)
            {
                int charsToTake = Math.Min(groupSize, numericText.Length - currentIndex);
                formattedText.Append(numericText.Substring(currentIndex, charsToTake));
                currentIndex += charsToTake;

                // Add a separator unless it's the last group
                if (currentIndex < numericText.Length && formattedText.Length < 20)
                    formattedText.Append('-');
            }
        }

        // Save the original caret position
        int oldCaretIndex = textBox.CaretIndex;

        // Update the text only if it has changed
        if (textBox.Text != formattedText.ToString())
        {
            textBox.Text = formattedText.ToString();

            // Adjust caret position dynamically
            int newCaretIndex = oldCaretIndex;

            // Prevent cursor from being stuck after separators
            if (oldCaretIndex > 0 && oldCaretIndex < formattedText.Length && formattedText[oldCaretIndex - 1] == '-')
            {
                newCaretIndex++;
            }

            textBox.CaretIndex = Math.Min(newCaretIndex, textBox.Text.Length);
        }
    }
    private void txtYattMFO_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox != null)
        {
            // Faqat raqamlarni qoldirish
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Eski noto'g'ri matnni to'g'rilash
            if (textBox.Text != numericText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = numericText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni o'rnida qoldirish
            }
        }
    }
    private void txtYattTelefon_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null)
        {
            // Faqat raqamlar va bo'sh joylarni qoldirish
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == ' ').ToArray());

            // Agar noto'g'ri belgilar kirgan bo'lsa, matnni yangilash
            if (textBox.Text != filteredText)
            {
                int caretIndex = textBox.CaretIndex; // Imlo ko'rsatkichni saqlash
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length); // Ko'rsatkichni to'g'ri joylash
            }
        }
    }

    // buttonlar
    private void btnRahbar_Click(object sender, RoutedEventArgs e)
    {
        spYurYattJis.Visibility = Visibility.Hidden;
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
            spYurYattJis.Visibility = Visibility.Hidden;
            spYurCutomer.Visibility = Visibility.Visible;
            spYattCutomer.Visibility = Visibility.Hidden;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility= Visibility.Hidden;
        }
        else if (CustomerInfo.CustomerId > 0 && !CustomerInfo.YurJshshir.Equals(""))
        {
            spYurYattJis.Visibility = Visibility.Hidden;
            spYurCutomer.Visibility = Visibility.Hidden;
            spYattCutomer.Visibility = Visibility.Visible;
            spJisCutomer.Visibility = Visibility.Hidden;
            spJisNew.Visibility = Visibility.Hidden;
            spQaytish.Visibility = Visibility.Hidden;
        }
    }

    private async void btnJisAdd_Click(object sender, RoutedEventArgs e)
    {
        UserCreationDto userCreationDto = new UserCreationDto();

        userCreationDto.LastName = txtFamiliya.Text.ToLower();
        userCreationDto.FirstName = txtIsmi.Text.ToLower();
        userCreationDto.Patronomyc = txtSharifi.Text.ToLower();
        userCreationDto.SeriaPasport = txtPasportSeria.Text.ToLower();
        userCreationDto.Address = txtJisAdres.Text.ToLower();
        userCreationDto.Phone = txtJisTelefon.Text;
        userCreationDto.TelegramPhone = txtJisTelegramRaqam.Text;
        userCreationDto.JSHSHIR = txtJisJSHSHIR.Text;
        userJshshir = txtJisJSHSHIR.Text;


        if (!dateOfBirthPicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Tug'ulgan sanani kiriting iltimos.");
            return;
        }
        userCreationDto.DateOfBirth = dateOfBirthPicker.SelectedDate.Value.ToUniversalTime();

        if (!dateOfIssuePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Pasport berilgan sanani kiriting iltimos.");
            return;
        }

        userCreationDto.DateOfIssue = dateOfIssuePicker.SelectedDate.Value.ToUniversalTime();
        if (!dateOfExpiryPicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Pasportni amal qilish sanani kiriting iltimos.");
            return;
        }
        userCreationDto.DateOfExpiry = dateOfExpiryPicker.SelectedDate.Value.ToUniversalTime();
        if (gender.Equals("Erkak"))
            userCreationDto.Gender = (Gender)0;
        else if (gender.Equals("Ayol"))
            userCreationDto.Gender = (Gender)1;


        if (userCreationDto.FirstName.Equals("") ||
            userCreationDto.LastName.Equals("") ||
            userCreationDto.SeriaPasport.Equals("") ||
            userCreationDto.Address.Equals("") ||
            userCreationDto.JSHSHIR.Equals("") ||
            userCreationDto.TelegramPhone.Equals("") ||
            userCreationDto.Phone.Equals(""))

            MessageBox.Show("Malumotni to'liq kiriting!");
        else
        {
            var result = this.userService.AddAsync(userCreationDto);

            if (!result.IsCompletedSuccessfully)
            {
                MessageBox.Show($" Saqlandi.");
                IsCreated = true;
            }
            else
                MessageBox.Show($"{"Saqlashda xatolik"}");
        }
    }

    private async void btnYattAdd_Click(object sender, RoutedEventArgs e)
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

        if (txtYattXisobRaqam.Text.Length != 23)
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
            MessageBox.Show("Ma'lumotni to'liq kiriting!");
        else
        {
            var result = customerService.ModifyAsync(customerUpdateDto);
             
            if (!result.IsCompletedSuccessfully)
            {
                MessageBox.Show($" Saqlandi.");
                IsCreated = true;
            }
            else
                MessageBox.Show($"{"Saqlashda xatolik"}");
        }
    }
}
