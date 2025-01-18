using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using DeLong_Desktop.Windows.Pirces;


namespace DeLong_Desktop.Companents;

/// <summary>
/// Interaction logic for PriceViewControl.xaml
/// </summary>
public partial class PriceViewControl : UserControl
{
    private readonly IServiceProvider services;

    public PriceViewControl(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private void OnAddClick(object sender, MouseButtonEventArgs e)
    {
        // Senderni aniqlash
        var clickedLabel = sender as Label;

        if (clickedLabel != null)
        {
            // Parent bo'lgan PriceViewControlni aniqlash
            var parentControl = FindParent<PriceViewControl>(clickedLabel);

            if (parentControl != null)
            {
                PriceInfo.PriceId = long.Parse(parentControl.tbPriceId.Text);
                PriceInfo.ArrivalPrice = decimal.Parse(parentControl.tbIncomePrice.Text);
                PriceInfo.SellingPrice = decimal.Parse(parentControl.tbSellPrice.Text);
                PriceInfo.Quatitiy = decimal.Parse(parentControl.tbQuantity.Text);
                PriceInfo.UnitOfMesure = parentControl.tbUnitOfMesure.Text;
            }
        }
        var addWindow = new PriceAddWindow(services);
        addWindow.ShowDialog(); // Yangi oynani modal tarzda ochish
    }

    // Yuqoridagi funksiya uchun yordamchi FindParent metodi
    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);

        while (parent != null && !(parent is T))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T;
    }

    private void OnEditClick(object sender, MouseButtonEventArgs e)
    {
        // Senderni aniqlash
        var clickedLabel = sender as Label;

        if (clickedLabel != null)
        {
            // Parent bo'lgan PriceViewControlni aniqlash
            var parentControl = FindParent<PriceViewControl>(clickedLabel);

            if (parentControl != null)
            {
                PriceInfo.PriceId = long.Parse(parentControl.tbPriceId.Text);
                PriceInfo.ArrivalPrice = decimal.Parse(parentControl.tbIncomePrice.Text);
                PriceInfo.SellingPrice = decimal.Parse(parentControl.tbSellPrice.Text);
                PriceInfo.Quatitiy = decimal.Parse(parentControl.tbQuantity.Text);
                PriceInfo.UnitOfMesure = parentControl.tbUnitOfMesure.Text;
            }
        }
        PriceEditWindow priceEditWindow = new PriceEditWindow(services);
        priceEditWindow.tbIncomePrice.Text = PriceInfo.ArrivalPrice.ToString();
        priceEditWindow.tbSellPrice.Text= PriceInfo.SellingPrice.ToString();
        priceEditWindow.tbUnitOfMesure.Text= PriceInfo.UnitOfMesure.ToString();
        priceEditWindow.ShowDialog();
    }
}
