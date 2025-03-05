namespace DeLong_Desktop.ApiService.DTOs.Debts;

public class DebtCreationDto
{
    public long SaleId { get; set; }  // Qaysi savdoga tegishli
    public decimal RemainingAmount { get; set; }  // To‘lanmagan qarz miqdori
    public bool IsSettled { get; set; } // Yangi xususiyat: Qarz to‘liq to‘langanmi?
    public DateTimeOffset DueDate { get; set; } // To‘lash muddati 🕒
}