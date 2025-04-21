using DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

namespace DeLong_Desktop.ApiService.DTOs.CreditorDebts;

public class CreditorDebtResultDto
{
    public long Id { get; set; }
    public long SupplierId { get; set; }
    public long TransactionId { get; set; }
    public string? SupplierName { get; set; } // Ta'minotchi nomi uchun (join orqali)
    public DateTimeOffset Date { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? Description { get; set; }
    public bool IsSettled { get; set; }
    public long BranchId { get; set; }
    public List<CreditorDebtPaymentResultDto> CreditorDebtPayments { get; set; } = new();
    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}