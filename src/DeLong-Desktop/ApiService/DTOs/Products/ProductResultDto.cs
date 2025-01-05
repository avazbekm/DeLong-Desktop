using DeLong_Desktop.ApiService.DTOs.Category;

namespace DeLong_Desktop.ApiService.DTOs.Products;

public class ProductResultDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public bool IsActive { get; set; }
}
