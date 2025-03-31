using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeResultDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long BranchId { get; set; }

    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)
}