using System.Windows.Input;
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
        var addWindow = new PriceAddWindow(services);
        addWindow.ShowDialog(); // Yangi oynani modal tarzda ochish
    }
}
