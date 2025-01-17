namespace DeLong_Desktop.ApiService.DTOs.Prices;

public class PriceCreationDto
{
    public decimal ArrivalPrice { get; set; }  // Kelish narxi
    public decimal SellingPrice { get; set; }  // Sotish narxi
    public string UnitOfMeasure { get; set; } = string.Empty; // kg,dona,karobka,litr
    public decimal Quantity { get; set; } // miqdori
    public long ProductId { get; set; }
}
