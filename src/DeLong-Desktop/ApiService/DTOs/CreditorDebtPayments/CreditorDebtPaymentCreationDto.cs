namespace DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

public class CreditorDebtPaymentCreationDto
{
    public long CreditorDebtId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    public string? Description { get; set; }
    public long BranchId { get; set; }
}