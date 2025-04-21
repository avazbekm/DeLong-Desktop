namespace DeLong_Desktop.ApiService.DTOs.CreditorDebts;

public class CreditorDebtUpdateDto
{
    public long Id { get; set; }
    public long? SupplierId { get; set; }
    public long TransactionId { get; set; }
    public DateTimeOffset? Date { get; set; }
    public decimal? RemainingAmount { get; set; }
    public string? Description { get; set; }
    public bool? IsSettled { get; set; }
    public long? BranchId { get; set; }
    public long? CreatedBy { get; set; }
}