using ControlzEx.Standard;

namespace DeLong_Desktop.ApiService.DTOs.Invoices;

public class InvoiceResultDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }

    public decimal TotalAmount { get; set; }
    public Status Status { get; set; } // Paid, Pending, Cancelled
}