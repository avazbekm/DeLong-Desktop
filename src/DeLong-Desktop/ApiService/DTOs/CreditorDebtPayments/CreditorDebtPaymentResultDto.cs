namespace DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

public class CreditorDebtPaymentResultDto
{
    public long Id { get; set; }
    public long CreditorDebtId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    public string? Description { get; set; }
    public long BranchId { get; set; }
    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}