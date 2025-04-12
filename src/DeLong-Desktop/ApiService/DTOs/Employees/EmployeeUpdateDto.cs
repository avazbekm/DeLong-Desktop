using DeLong_Desktop.ApiService.DTOs.Enums;
using System.ComponentModel.DataAnnotations;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeUpdateDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)
    public string Password { get; set; } = string.Empty; // Parol (hashlangan)

    public long BranchId { get; set; } // hodim qaysi filial bilan ishlashini aniqlash uchun
}