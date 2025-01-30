namespace DeLong_Desktop.Pages.SalesPractice;

public class ProductItem
{
    public long PriceId { get; set; }
    public int SerialNumber { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity; // Umumiy summasi avtomatik hisoblanadi
}

