namespace DeLong_Desktop.Pages.SalesPractice;

public class SaleItemPrintModel
{
    public int SerialNumber { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
