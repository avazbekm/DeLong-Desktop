using System.Text;
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
    private readonly ICustomerService customerService;
    private readonly IServiceProvider services;

    public CustomerAddWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.userService = services.GetRequiredService<IUserService>();
        this.customerService = services.GetRequiredService<ICustomerService>();
    }

    private void rbtnYurdik_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Visible;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Hidden;
        spYurYattJis.Visibility = Visibility.Visible;
        spQaytish.Visibility= Visibility.Hidden;
    }

    private void rbtnYaTT_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Visible;
        spYurYattJis.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Hidden;
    }

    private void rbtnJismoniy_Checked(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Visible;
        spYattCutomer.Visibility = Visibility.Hidden;
        spYurYattJis.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Hidden;
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
        if(txtJisJSHSHIR.Text.Equals(""))
        {
            MessageBox.Show("JSHSHIRni kiriting iltimos");
            return;
        }
        userCreationDto.JSHSHIR = long.Parse(txtJisJSHSHIR.Text);
        
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
         userCreationDto.TelegramPhone.Equals("") ||
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

    private void btnYurAdd_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnRahbar_Click(object sender, RoutedEventArgs e)
    {
        spYurYattJis.Visibility = Visibility.Hidden;
        spYurCutomer.Visibility = Visibility.Hidden;
        spJisCutomer.Visibility = Visibility.Visible;
        spYattCutomer.Visibility = Visibility.Hidden;
        spQaytish.Visibility = Visibility.Visible;
    }

    private void btnQaytish_Click(object sender, RoutedEventArgs e)
    {
        spYurCutomer.Visibility = Visibility.Visible;
        spJisCutomer.Visibility = Visibility.Hidden;
        spYattCutomer.Visibility = Visibility.Hidden;
        spYurYattJis.Visibility = Visibility.Visible;
        spQaytish.Visibility = Visibility.Hidden;
    }

    private void txtJisJSHSHIR_TextChanged(object sender, TextChangedEventArgs e)
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

    private void txtYurINN_TextChanged(object sender, TextChangedEventArgs e)
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



}
