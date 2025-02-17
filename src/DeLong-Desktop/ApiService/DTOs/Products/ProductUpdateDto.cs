namespace DeLong_Desktop.ApiService.DTOs.Products;

public class ProductUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? MinStockLevel { get; set; }  // minimal qoldiqni belgilab qo'yish
    public long CategoryId { get; set; }
    public bool IsActive { get; set; }
}
