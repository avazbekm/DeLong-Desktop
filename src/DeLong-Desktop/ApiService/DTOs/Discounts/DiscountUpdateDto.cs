namespace DeLong_Desktop.ApiService.DTOs.Discounts;

public class DiscountUpdateDto
{
    public long Id { get; set; } // Chegirma ID’si
    public long SaleId { get; set; } // Qaysi sotuvga tegishli
    public decimal Amount { get; set; } // Chegirma summasi
}