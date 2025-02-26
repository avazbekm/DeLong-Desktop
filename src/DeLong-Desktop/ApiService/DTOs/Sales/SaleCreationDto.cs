using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Payments;

namespace DeLong_Desktop.ApiService.DTOs.Sales;

public class SaleCreationDto
{
    public long? CustomerId { get; set; }
    public long? UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PaymentCreationDto> Payments { get; set; } = new();
    public List<DebtCreationDto> Debts { get; set; } = new();
}