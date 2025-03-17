namespace DeLong_Desktop.ApiService.DTOs.CashWarehouses;

public class CashWarehouseCreationDto
{
    public decimal UzsBalance { get; set; } = 0; // Ombordagi so‘m (default 0)
    public decimal UzpBalance { get; set; } = 0; // Plastik qoldig‘i (default 0)
    public decimal UsdBalance { get; set; } = 0; // Ombordagi dollar (default 0)
}