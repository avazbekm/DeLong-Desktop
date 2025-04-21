namespace DeLong_Desktop.Pages.InputHistories;

public class PaymentItem
{
    public DateTimeOffset PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
}