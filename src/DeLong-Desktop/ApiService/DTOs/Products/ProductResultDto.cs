using DeLong_Desktop.ApiService.DTOs.Category;

namespace DeLong_Desktop.ApiService.DTOs.Products;

public class ProductResultDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductSign { get; set; } = string.Empty;
    public decimal? MinStockLevel { get; set; }  // minimal qoldiqni belgilab qo'yish
    public long CategoryId { get; set; }
    public bool IsActive { get; set; }
    public long CreatedBy { get; set; }
}
