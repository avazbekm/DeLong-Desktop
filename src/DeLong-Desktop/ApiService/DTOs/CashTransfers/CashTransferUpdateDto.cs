namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferUpdateDto
{
    public long Id { get; set; } // Yangilanadigan o‘tkazma ID si
    public long CashRegisterId { get; set; } // Qaysi kassaga bog‘liq

    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
}
