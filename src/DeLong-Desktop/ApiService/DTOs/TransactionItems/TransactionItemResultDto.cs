namespace DeLong_Desktop.ApiService.DTOs.TransactionItems;

public class TransactionItemResultDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // Mahsulot nomi (qo‘shimcha ma’lumot)
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal PriceProduct { get; set; }
    public string Comment { get; set; } = string.Empty; // Har mahsulot uchun izoh
}