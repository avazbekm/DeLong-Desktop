using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.DTOs.Suppliers;

namespace DeLong_Desktop.ApiService.DTOs.Prices;

public class PriceResultDto
{
    public long Id { get; set; }

    public long? SupplierId { get; set; }
    public SupplierResultDto? Supplier { get; set; }

    public long ProductId { get; set; }
    public ProductResultDto Product { get; set; }
    public long CreatedBy { get; set; }

    public decimal CostPrice { get; set; }  // Kelish narxi
    public decimal SellingPrice { get; set; }  // Sotish narxi
    public string UnitOfMeasure { get; set; } = string.Empty; // kg,dona,karobka,litr
    public decimal Quantity { get; set; } // miqdori
}

