namespace DeLong_Desktop.ApiService.DTOs;

public class ReturnProductUpdateDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }     // Jismoniy shaxsdan mahsulot qaytyapti
    public long? CustomerId { get; set; }   // Yuridik shaxsdan mahsulot qaytyapti
    public long SaleId { get; set; }      // Savdo qilgan mahsulotlar ro‘yxati olish uchun ID
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quatity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal ReturnSumma { get; set; }
    public string Reason { get; set; } = string.Empty; // Qaytish sababi
}