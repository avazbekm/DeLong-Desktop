using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeLong_Desktop.Windows.Pirces;

namespace DeLong_Desktop.Companents;

public partial class PriceViewControl : UserControl
{
    private readonly IServiceProvider services;
    public event EventHandler PriceUpdated;

    public PriceViewControl(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private void OnAddClick(object sender, MouseButtonEventArgs e)
    {
        var clickedLabel = sender as Label;
        if (clickedLabel != null)
        {
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
        addWindow.PriceModified += (s, e) => PriceUpdated?.Invoke(this, EventArgs.Empty); // Yangilash signalini uzatish
        addWindow.ShowDialog();
    }

    private void OnEditClick(object sender, MouseButtonEventArgs e)
    {
        var clickedLabel = sender as Label;
        if (clickedLabel != null)
        {
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

        PriceEditWindow priceEditWindow = new PriceEditWindow(services)
        {
            tbIncomePrice = { Text = PriceInfo.ArrivalPrice.ToString() },
            tbSellPrice = { Text = PriceInfo.SellingPrice.ToString() },
            tbUnitOfMesure = { Text = PriceInfo.UnitOfMesure }
        };
        priceEditWindow.PriceModified += (s, e) => PriceUpdated?.Invoke(this, EventArgs.Empty); // Yangilash signalini uzatish
        priceEditWindow.ShowDialog();
    }

    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);
        while (parent != null && !(parent is T))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as T;
    }
}