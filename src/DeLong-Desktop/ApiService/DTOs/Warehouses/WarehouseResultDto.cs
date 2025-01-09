namespace DeLong_Desktop.ApiService.DTOs.Warehouses;

class WarehouseResultDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
}