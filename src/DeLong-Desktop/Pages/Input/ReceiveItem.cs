namespace DeLong_Desktop.Pages.Input;

public class ReceiveItem
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } =string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TotalAmount { get; set; }
}
