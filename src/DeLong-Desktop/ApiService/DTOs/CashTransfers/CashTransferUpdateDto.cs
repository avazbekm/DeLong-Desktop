namespace DeLong_Desktop.ApiService.DTOs.CashTransfers;

public class CashTransferUpdateDto
{
    public long Id { get; set; } // Yangilanadigan o‘tkazma ID si

    public decimal UzsBalance { get; set; }  // Yangi so‘m miqdori
    public decimal UzpBalance { get; set; }  // Yangi plastik miqdori
    public decimal UsdBalance { get; set; }  // Yangi dollar miqdori
    public decimal DebtAmount { get; set; }  // Yangi nasiya summasi (agar kerak bo‘lsa)

    public string TransferType { get; set; } = string.Empty; // Yangi transfer turi
}
