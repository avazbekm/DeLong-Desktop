namespace DeLong_Desktop.Pages.SaleHistory;

public class SaleDisplayItem
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
