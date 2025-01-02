namespace DeLong_Desktop.ApiService.DTOs.Products;

public class ProductCreationDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public bool IsActive { get; set; }
}
