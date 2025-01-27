namespace DeLong_Desktop.ApiService.DTOs.InvoiceItems;

public class InvoiceItemResultDto
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
