using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.DollarKurs;

/// <summary>
/// Interaction logic for DollarBuyWindow.xaml
/// </summary>
public partial class DollarBuyWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IKursDollarService kursDollarService;

    public DollarBuyWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.kursDollarService = services.GetRequiredService<IKursDollarService>();
        tbDollarKurs.Focus();
    }

    private void btnAddDollarKurs_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void tbDollarKurs_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
            if (tbDollarKurs.Text.Length > 0)
            {
                var dollarKurs = await this.kursDollarService.RetrieveByIdAsync();
                tbDollarOlishKurs.Text = (decimal.Parse(tbDollarKurs.Text) * dollarKurs.AdmissionDollar).ToString("N2");
            }
            else
                tbDollarOlishKurs.Text = "";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void tbDollarOlishKurs_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
