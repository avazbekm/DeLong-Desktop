using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.DTOs.Suppliers;

namespace DeLong_Desktop.Pages.Reports;

class PriceItem
{
    public int TartibRaqam { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; } // miqdori
    public decimal CostPrice { get; set; }  // Kelish narxi
    public string UnitOfMeasure { get; set; } = string.Empty; // kg,dona,karobka,litr
    public decimal TotalPrice => CostPrice * Quantity;
}
