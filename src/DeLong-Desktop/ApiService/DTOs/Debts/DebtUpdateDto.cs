namespace DeLong_Desktop.ApiService.DTOs.Debts;

public class DebtUpdateDto
{
    public long Id { get; set; }  // Qarzning ID’si
    public long SaleId { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTimeOffset DueDate { get; set; } // To‘lash muddati 🕒
}