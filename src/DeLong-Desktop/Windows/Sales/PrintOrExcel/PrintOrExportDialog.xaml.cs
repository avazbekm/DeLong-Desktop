using System.Windows;

namespace DeLong_Desktop.Windows.Sales.PrintOrExcel;

/// <summary>
/// Interaction logic for PrintOrExportDialog.xaml
/// </summary>
public partial class PrintOrExportDialog : Window
{
    public bool PrintSelected { get; private set; }

    public PrintOrExportDialog()
    {
        InitializeComponent();
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        PrintSelected = true;
        DialogResult = true;
        Close();
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        PrintSelected = false;
        DialogResult = true;
        Close();
    }
}
