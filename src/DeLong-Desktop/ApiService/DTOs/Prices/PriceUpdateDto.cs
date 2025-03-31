namespace DeLong_Desktop.ApiService.DTOs.Prices;

public class PriceUpdateDto
{
    public long Id { get; set; }
    public long? SupplierId { get; set; }
    public long ProductId { get; set; }
    public decimal CostPrice { get; set; }  // Kelish narxi
    public decimal SellingPrice { get; set; }  // Sotish narxi
    public string UnitOfMeasure { get; set; } = string.Empty; // kg,dona,karobka,litr
    public decimal Quantity { get; set; } // miqdori
}

