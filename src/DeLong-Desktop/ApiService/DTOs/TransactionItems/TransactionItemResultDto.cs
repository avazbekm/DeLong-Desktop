﻿namespace DeLong_Desktop.ApiService.DTOs.TransactionItems;

public class TransactionItemResultDto
{
    public long Id { get; set; }
    public long TransactionId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // Mahsulot nomi (qo‘shimcha ma’lumot)
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal PriceProduct { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}