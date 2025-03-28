﻿using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Employees;

public class EmployeeCreationDto
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("warehouseId")]
    public long WarehouseId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;  // Foydalanuvchi nomi (Login)

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty; // Parol (hashlangan)
}