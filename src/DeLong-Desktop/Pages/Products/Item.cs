namespace DeLong_Desktop.Pages.Products;

public class Item
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string UnitOfMesure { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
