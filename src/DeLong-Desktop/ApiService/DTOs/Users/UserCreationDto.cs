using System.Text.Json.Serialization;
using DeLong_Desktop.ApiService.DTOs.Enums;

namespace DeLong_Desktop.ApiService.DTOs.Users;

public class UserCreationDto
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? Patronomyc { get; set; } = string.Empty;
    public string? SeriaPasport { get; set; } = string.Empty;
    public DateTimeOffset? DateOfBirth { get; set; }
    public DateTimeOffset? DateOfIssue { get; set; } // pasport berilgan sana
    public DateTimeOffset? DateOfExpiry { get; set; } // Amal qilish muddati
    public Gender Gender { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? JSHSHIR { get; set; } = string.Empty;
    public Role Role { get; set; }
    public long BranchId { get; set; }
    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)
    public string Password { get; set; } = string.Empty; // Parol (hashlangan)
}