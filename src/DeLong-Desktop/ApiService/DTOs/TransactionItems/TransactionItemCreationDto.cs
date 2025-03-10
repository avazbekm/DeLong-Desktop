namespace DeLong_Desktop.ApiService.DTOs.TransactionItems;

public class TransactionItemCreationDto
{
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal PriceProduct { get; set; }
    public string Comment { get; set; } = string.Empty; // Har mahsulot uchun ixtiyoriy izoh
}
