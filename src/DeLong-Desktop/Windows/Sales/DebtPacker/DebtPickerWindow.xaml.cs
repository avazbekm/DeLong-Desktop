using System.Windows;

namespace DeLong_Desktop.Windows.Sales.DebtPacker;

/// <summary>
/// Interaction logic for DebtPickerWindow.xaml
/// </summary>
public partial class DebtPickerWindow : Window
{
    public DateTime? SelectedDate { get; private set; } // Foydalanuvchi tanlagan sana
    public DebtPickerWindow()
    {
        InitializeComponent();
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (dpDueDate.SelectedDate == null)
        {
            MessageBox.Show("Iltimos, qarz to‘lash sanasini tanlang!", "Ogohlantirish", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DateTime selectedDate = dpDueDate.SelectedDate.Value;
        if (selectedDate < DateTime.Today)
        {
            MessageBox.Show("To‘lov sanasi bugungi sanadan kichik bo‘lishi mumkin emas!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        SelectedDate = selectedDate;
        DialogResult = true;
        Close();
    }

}
