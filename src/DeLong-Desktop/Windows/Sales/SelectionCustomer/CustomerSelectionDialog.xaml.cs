using System.Windows;

namespace DeLong_Desktop.Windows.Sales.SelectionCustomer;

/// <summary>
/// Interaction logic for CustomerSelectionDialog.xaml
/// </summary>
public partial class CustomerSelectionDialog : Window
{
    public bool SelectCustomer { get; private set; }

    public CustomerSelectionDialog()
    {
        InitializeComponent();
    }

    private void SelectButton_Click(object sender, RoutedEventArgs e)
    {
        SelectCustomer = true;
        DialogResult = true;
        Close();
    }

    private void SkipButton_Click(object sender, RoutedEventArgs e)
    {
        SelectCustomer = false;
        DialogResult = true;
        Close();
    }
}
