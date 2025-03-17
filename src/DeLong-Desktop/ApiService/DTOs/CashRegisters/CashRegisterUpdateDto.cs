namespace DeLong_Desktop.ApiService.DTOs.CashRegisters;

public class CashRegisterUpdateDto
{
    public long Id { get; set; } // Yangilanadigan kassa ID si

    public decimal UzsBalance { get; set; }  // Yangi so‘m qoldig‘i
    public decimal UzpBalance { get; set; }  // Yangi plastik qoldig‘i
    public decimal UsdBalance { get; set; }  // Yangi dollar qoldig‘i
    public decimal DebtAmount { get; set; } // Yangi nasiya summasi

    public DateTimeOffset? ClosedAt { get; set; } // Kassa yopilish vaqti (ixtiyoriy)
}