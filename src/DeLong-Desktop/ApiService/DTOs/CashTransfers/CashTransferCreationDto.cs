using DeLong_Desktop.ApiService.DTOs.Enums;

namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferCreationDto
{
    public long CashRegisterId { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public CashTransferType TransferType { get; set; } // Yangi qo‘shildi
    public DateTimeOffset TransferDate { get; set; }
}