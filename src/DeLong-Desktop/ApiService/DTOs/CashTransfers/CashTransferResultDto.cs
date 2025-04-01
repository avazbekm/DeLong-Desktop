using DeLong_Desktop.ApiService.DTOs.Enums;

namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferResultDto
{
    public long Id { get; set; } // O‘tkazma ID si
    public long CashRegisterId { get; set; } // Qaysi kassaga bog‘liq
    public string CashRegisterInfo { get; set; } = string.Empty; // Kassir yoki kassa haqida qisqa ma’lumot (masalan, "Kassir1 - Warehouse1")

    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public CashTransferType TransferType { get; set; } // Yangi qo‘shildi
    public DateTimeOffset TransferDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } // O‘tkazma qachon qilingan
    public DateTimeOffset? UpdatedAt { get; set; } // O‘tkazma qachon yangilangan (agar bo‘lsa)
}