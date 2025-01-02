namespace DeLong_Desktop.Pages.Products;

public class Item
{
    public long Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string UnitOfMesure { get; set; } = string.Empty;
    public decimal SellingPrice { get; set; }
    public decimal SellingVol { get; set; }
}
