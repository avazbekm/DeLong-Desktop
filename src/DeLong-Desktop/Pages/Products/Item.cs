namespace DeLong_Desktop.Pages.Products;

public class Item
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MahsulotBelgisi { get; set; } = string.Empty;
    public Decimal Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long CategoryId { get; set; }

}
