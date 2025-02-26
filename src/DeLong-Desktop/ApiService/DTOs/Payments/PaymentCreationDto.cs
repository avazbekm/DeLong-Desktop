using DeLong_Desktop.ApiService.DTOs.Enums;
using DocumentFormat.OpenXml.Bibliography;

namespace DeLong_Desktop.ApiService.DTOs.Payments;

public class PaymentCreationDto
{
    public long SaleId { get; set; }  // Qaysi savdoga tegishli
    public decimal Amount { get; set; }  // To‘lov summasi
    public PaymentType Type { get; set; }  // To‘lov turi (Naqd, Plastik, Bank o‘tkazmasi, Nasiya)
}