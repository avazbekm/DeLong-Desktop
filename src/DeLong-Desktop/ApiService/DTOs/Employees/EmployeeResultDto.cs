using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeResultDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("warehouseId")]
    public long WarehouseId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)
}