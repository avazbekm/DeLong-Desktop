namespace DeLong_Desktop.ApiService.DTOs.Suppliers;

public class SupplierUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}