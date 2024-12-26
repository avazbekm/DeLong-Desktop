using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Customers;

class CustomerCreationDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("inn")]
    public int INN { get; set; }

    [JsonPropertyName("jshshir")]
    public string JSHSHIR { get; set; } = string.Empty;
   
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("mfo")]
    public string MFO { get; set; } = string.Empty;

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
}
