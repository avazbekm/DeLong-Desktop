using DeLong_Desktop.ApiService.DTOs.Users;
using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Customers;

public class CustomerResultDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("inn")]
    public int INN { get; set; }

    [JsonPropertyName("mfo")]
    public string MFO { get; set; } = string.Empty;

    [JsonPropertyName("jshshir")]
    public string JSHSHIR { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("bankAccount")]
    public string BankAccount { get; set; } = string.Empty;

    [JsonPropertyName("bankName")]
    public string BankName { get; set; } = string.Empty;

    [JsonPropertyName("okonx")]
    public string OKONX { get; set; } = string.Empty;

    [JsonPropertyName("yurAddress")]
    public string YurAddress { get; set; } = string.Empty;

    // firma rahbari malumotlari
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("user")]
    public UserResultDto User { get; set; }
}
