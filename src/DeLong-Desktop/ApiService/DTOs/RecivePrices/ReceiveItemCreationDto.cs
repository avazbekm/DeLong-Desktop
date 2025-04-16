namespace DeLong_Desktop.ApiService.DTOs.RecivePrices;

public class ReceiveItemCreationDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public long SupplierId { get; set; }
    public decimal SellingPrice { get; set; }
    public bool IsUpdate { get; set; } = false; // true: update, false: add
    public long PriceId { get; set; } // PriceAddWindow uchun Price.Id
}
