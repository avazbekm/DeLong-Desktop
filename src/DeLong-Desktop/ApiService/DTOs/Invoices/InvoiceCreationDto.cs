using ControlzEx.Standard;

namespace DeLong_Desktop.ApiService.DTOs.Invoices;

class InvoiceCreationDto
{
    public long CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public Status Status { get; set; } // Paid, Pending, Cancelled
}