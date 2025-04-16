namespace DeLong_Desktop.ApiService.DTOs.CreditorDebts;

public class CreditorDebtCreationDto
{
    public long SupplierId { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? Description { get; set; }
    public long BranchId { get; set; }
}