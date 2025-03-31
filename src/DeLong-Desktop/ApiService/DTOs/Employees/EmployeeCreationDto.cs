using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeCreationDto
{
    public long UserId { get; set; }

    public long BranchId { get; set; }

    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)

    public string Password { get; set; } = string.Empty; // Parol (hashlangan)
}