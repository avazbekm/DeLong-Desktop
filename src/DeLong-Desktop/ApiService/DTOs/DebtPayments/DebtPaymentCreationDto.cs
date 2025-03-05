namespace DeLong_Desktop.ApiService.DTOs.DebtPayments;

public class DebtPaymentCreationDto
{
    public long DebtId { get; set; }  // Qaysi qarzga tegishli
    public decimal Amount { get; set; }  // To‘langan summa
    public DateTimeOffset PaymentDate { get; set; } = DateTime.UtcNow;  // To‘lov sanasi
    public string PaymentMethod { get; set; } = string.Empty; // Yangi: "Cash", "Card", "Dollar"

}