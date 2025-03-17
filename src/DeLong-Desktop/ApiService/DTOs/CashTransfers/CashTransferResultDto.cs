namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferResultDto
{
    public long Id { get; set; } // O‘tkazma ID si
    public long CashRegisterId { get; set; } // Qaysi kassaga bog‘liq
    public string CashRegisterInfo { get; set; } = string.Empty; // Kassir yoki kassa haqida qisqa ma’lumot (masalan, "Kassir1 - Warehouse1")

    public decimal UzsBalance { get; set; }  // O‘tkazilgan so‘m miqdori
    public decimal UzpBalance { get; set; }  // O‘tkazilgan plastik miqdori
    public decimal UsdBalance { get; set; }  // O‘tkazilgan dollar miqdori
    public decimal DebtAmount { get; set; }  // Nasiya summasi (agar kerak bo‘lsa)

    public string TransferType { get; set; } = string.Empty; // "FromStorage" yoki "ToStorage"
    public DateTimeOffset CreatedAt { get; set; } // O‘tkazma qachon qilingan
    public DateTimeOffset? UpdatedAt { get; set; } // O‘tkazma qachon yangilangan (agar bo‘lsa)
}