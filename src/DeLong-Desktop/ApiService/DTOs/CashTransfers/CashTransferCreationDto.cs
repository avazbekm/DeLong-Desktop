namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferCreationDto
{
    public long CashRegisterId { get; set; } // Qaysi kassaga bog‘liq

    public decimal UzsBalance { get; set; }  // O‘tkazilgan so‘m miqdori
    public decimal UzpBalance { get; set; }  // O‘tkazilgan plastik miqdori
    public decimal UsdBalance { get; set; }  // O‘tkazilgan dollar miqdori
    public decimal DebtAmount { get; set; }  // Nasiya summasi (agar kerak bo‘lsa)

    public string TransferType { get; set; } = string.Empty; // "FromStorage" yoki "ToStorage"
}