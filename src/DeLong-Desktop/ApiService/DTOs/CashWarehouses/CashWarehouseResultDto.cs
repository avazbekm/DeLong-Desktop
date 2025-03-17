namespace DeLong_Desktop.ApiService.DTOs.CashWarehouses;

public class CashWarehouseResultDto
{
    public long Id { get; set; } // Ombor ID si

    public decimal UzsBalance { get; set; } // Ombordagi so‘m qoldig‘i
    public decimal UzpBalance { get; set; } // Plastik qoldig‘i
    public decimal UsdBalance { get; set; } // Ombordagi dollar qoldig‘i

    public DateTimeOffset CreatedAt { get; set; } // Ombor qachon yaratilgan
    public DateTimeOffset? UpdatedAt { get; set; } // Ombor qachon yangilangan (agar bo‘lsa)

    public List<long> CashRegisterIds { get; set; } = new(); // Bog‘langan kassalar ID’lari
}