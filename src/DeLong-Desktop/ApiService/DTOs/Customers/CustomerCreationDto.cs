using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Customers;

class CustomerCreationDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("inn")]
    public int INN { get; set; }

    [JsonPropertyName("mfo")]
    public string MFO { get; set; } = string.Empty;

    [JsonPropertyName("bankAccount")]
    public long BankAccount { get; set; }

    [JsonPropertyName("bankName")]
    public string BankName { get; set; } = string.Empty;

    [JsonPropertyName("okonx")]
    public string OKONX { get; set; } = string.Empty;

    [JsonPropertyName("yurAddress")]
    public string YurAddress { get; set; } = string.Empty;

    // firma rahbari malumotlari
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
}
