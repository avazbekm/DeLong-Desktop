namespace DeLong_Desktop.ApiService.DTOs.DebtPayments;

public class DebtPaymentResultDto
{
    public long Id { get; set; }  // To‘lov ID’si
    public long DebtId { get; set; }
    public decimal Amount { get; set; }  // To‘langan summa
    public DateTime PaymentDate { get; set; }  // To‘lov sanasi
    public DateTime CreatedAt { get; set; }  // Qachon yaratilgan
}