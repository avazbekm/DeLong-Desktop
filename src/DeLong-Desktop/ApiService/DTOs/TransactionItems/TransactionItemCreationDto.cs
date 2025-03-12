namespace DeLong_Desktop.ApiService.DTOs.TransactionItems;

public class TransactionItemCreationDto
{
    public long TransactionId { get; set; }
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal PriceProduct { get; set; }
}
