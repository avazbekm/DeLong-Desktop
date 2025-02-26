using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.DTOs.Payments;

namespace DeLong_Desktop.ApiService.DTOs.Sales;

public class SaleUpdateDto
{
    public long Id { get; set; }
    public long? CustomerId { get; set; }
    public long? UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PaymentUpdateDto> Payments { get; set; } = new();
    public List<DebtUpdateDto> Debts { get; set; } = new();
}