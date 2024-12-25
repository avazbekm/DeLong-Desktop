using DeLong_Desktop.ApiService.DTOs.Enums;
using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Users;

public class UserResultDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("patronomyc")]
    public string Patronomyc { get; set; } = string.Empty;

    [JsonPropertyName("seriaPasport")]
    public string SeriaPasport { get; set; } = string.Empty;

    [JsonPropertyName("dateOfBirth")]
    public DateTimeOffset DateOfBirth { get; set; }

    [JsonPropertyName("dateOfIssue")]
    public DateTimeOffset DateOfIssue { get; set; } // pasport berilgan sana
    
    [JsonPropertyName("dateOfExpiry")]
    public DateTimeOffset DateOfExpiry { get; set; } // Amal qilish muddati
    
    [JsonPropertyName("gender")]
    public Gender Gender { get; set; }
    
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [JsonPropertyName("telegramPhone")]
    public string TelegramPhone { get; set; } = string.Empty;
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("jshshir")]
    public string JSHSHIR { get; set; } = string.Empty;
}

