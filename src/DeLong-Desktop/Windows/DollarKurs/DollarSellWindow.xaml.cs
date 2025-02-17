using System.Windows;
using System.Windows.Controls;
using DeLong_Desktop.ApiService.Helpers;
using DeLong_Desktop.ApiService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Windows.DollarKurs;

/// <summary>
/// Interaction logic for DollarSellWindow.xaml
/// </summary>
public partial class DollarSellWindow : Window
{
    private readonly IServiceProvider services;
    private readonly IKursDollarService kursDollarService;
    public DollarSellWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.kursDollarService = services.GetRequiredService<IKursDollarService>();
        tbDollarSotishKurs.Focus();
    }

    private void btnSellDollar_Click(object sender, RoutedEventArgs e)
    {

    }

    private void tbSomBerishKurs_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private async void tbDollarSotishKurs_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            ValidationHelper.ValidateOnlyNumberInput(sender as TextBox);
            if (tbDollarSotishKurs.Text.Length > 0)
            {
                var dollarKurs = await this.kursDollarService.RetrieveByIdAsync();
                tbSomBerishKurs.Text = (dollarKurs.SellingDollar * decimal.Parse(tbDollarSotishKurs.Text)).ToString("N2");
            }
            else
                tbSomBerishKurs.Text = "";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
