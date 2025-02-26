namespace DeLong_Desktop.ApiService.DTOs.DebtPayments;

public class DebtPaymentUpdateDto
{
    public long Id { get; set; }  // To‘lov ID’si
    public long DebtId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
