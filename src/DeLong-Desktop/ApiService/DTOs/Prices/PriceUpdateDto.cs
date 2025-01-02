namespace DeLong_Desktop.ApiService.DTOs.Prices;

public class PriceUpdateDto
{
    public long Id { get; set; }
    public decimal ArrivalPrice { get; set; }  // Kelish narxi
    public decimal SellingPrice { get; set; }  // Sotish narxi
    public string UnitOfMeasure { get; set; } = string.Empty; // kg,dona,karobka,litr
    public int Quantity { get; set; } // miqdori

    public long ProductId { get; set; }
}

