using System.ComponentModel.DataAnnotations;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeUpdateDto
{
    public long Id { get; set; }
    public long UserId { get; set; }

    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)

    [MinLength(8, ErrorMessage = "Parol kamida 8 ta belgidan iborat bo'lishi kerak")]
    public string Password { get; set; } = string.Empty; // Parol (hashlangan)
}