namespace DeLong_Desktop.Pages.InputHistories;

public class HistoryItem
{
    public long DebtId { get; set; }
    public long TransactionId { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; }
}
