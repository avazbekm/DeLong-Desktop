namespace DeLong_Desktop.ApiService.DTOs.CashWarehouses;

public class CashWarehouseUpdateDto
{
    public long Id { get; set; } // Yangilanadigan ombor ID si

    public decimal UzsBalance { get; set; } // Yangi so‘m qoldig‘i
    public decimal UzpBalance { get; set; } // Yangi plastik qoldig‘i
    public decimal UsdBalance { get; set; } // Yangi dollar qoldig‘i
}