using System.Windows.Controls;
using System.Windows.Media;
using DeLong_Desktop.ApiService.Interfaces;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace DeLong_Desktop.Pages.Reports
{
    public partial class ProductWiewPage : Page
    {
        private readonly IProductService _productService;
        public SeriesCollection ProductValues { get; set; }
        public List<string> ProductNames { get; set; }

        public ProductWiewPage(IServiceProvider services)
        {
            InitializeComponent();
            _productService = services.GetRequiredService<IProductService>();

            LoadProductChartData();
            DataContext = this;
        }

        private async void LoadProductChartData()
        {
            var products = await _productService.RetrieveAllAsync();
            if (products == null || !products.Any()) return;

            ProductNames = products.Select(p => p.Name).ToList();
            var values = new ChartValues<int>(products.Select(p => (int)p.MinStockLevel.GetValueOrDefault()));

            ProductValues = new SeriesCollection();

            for (int i = 0; i < ProductNames.Count; i++)
            {
                ProductValues.Add(new ColumnSeries
                {
                    Title = ProductNames[i],
                    Values = new ChartValues<int> { values[i] },
                    Fill = new SolidColorBrush(Color.FromRgb((byte)(50 + i * 30), (byte)(100 + i * 20), (byte)(200 - i * 10)))
                });
            }

            DataContext = this;
        }
    }
}
