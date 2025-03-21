namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferCreationDto
{
    public long CashRegisterId { get; set; } // Qaysi kassaga bog‘liq
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTimeOffset TransferDate { get; set; }
}