namespace DeLong_Desktop.ApiService.DTOs.Products;

public class ProductCreationDto
{
    public string Name { get; set; } = string.Empty;
    public string ProductSign { get; set; } = string.Empty;
    public decimal? MinStockLevel { get; set; }  // minimal qoldiqni belgilab qo'yish
    public long CategoryId { get; set; }
    public bool IsActive { get; set; }
}
