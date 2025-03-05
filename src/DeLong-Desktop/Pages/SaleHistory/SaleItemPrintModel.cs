namespace DeLong_Desktop.Pages.SaleHistory;

public class SaleItemPrintModel
{
    public string SerialNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}