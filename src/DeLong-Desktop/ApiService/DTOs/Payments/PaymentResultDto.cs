namespace DeLong_Desktop.ApiService.DTOs.Payments;

public class PaymentResultDto
{
    public long Id { get; set; }  // To‘lov ID’si
    public long SaleId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }  // Enumni string sifatida yuborish
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }  // To‘lov qachon amalga oshirilgan
}