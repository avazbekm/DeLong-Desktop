namespace DeLong_Desktop.Pages.AdditianalOperations;

public class DebtItem
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal RemainingAmount { get; set; }
    public string DueDate { get; set; } = string.Empty;// Ko‘rsatish uchun (string)
    public DateTimeOffset DueDateValue { get; set; } // Tartiblash uchun (DateTimeOffset)
}
