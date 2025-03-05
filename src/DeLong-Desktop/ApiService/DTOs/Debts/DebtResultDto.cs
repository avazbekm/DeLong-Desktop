using DeLong_Desktop.ApiService.DTOs.DebtPayments;

namespace DeLong_Desktop.ApiService.DTOs.Debts;

public class DebtResultDto
{
    public long Id { get; set; }  // Qarz ID’si
    public long SaleId { get; set; }
    public decimal RemainingAmount { get; set; }  // Hali to‘lanmagan qarz miqdori
    public bool IsSettled { get; set; } // Yangi xususiyat: Qarz to‘liq to‘langanmi?
    public List<DebtPaymentResultDto> DebtPayments { get; set; } = new();  // Qarz bo‘yicha to‘lovlar
    public DateTimeOffset DueDate { get; set; } // To‘lash muddati 🕒
}