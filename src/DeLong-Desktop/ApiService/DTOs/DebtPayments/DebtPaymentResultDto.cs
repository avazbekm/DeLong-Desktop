namespace DeLong_Desktop.ApiService.DTOs.DebtPayments;

public class DebtPaymentResultDto
{
    public long Id { get; set; }  // To‘lov ID’si
    public long DebtId { get; set; }
    public decimal Amount { get; set; }  // To‘langan summa
    public DateTimeOffset PaymentDate { get; set; }  // To‘lov sanasi
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }  // Qachon yaratilgan
    public string PaymentMethod { get; set; } = string.Empty; // Yangi: "Cash", "Card", "Dollar"

}