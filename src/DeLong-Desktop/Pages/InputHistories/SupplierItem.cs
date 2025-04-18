namespace DeLong_Desktop.Pages.InputHistories;

public class SupplierItem
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public override string ToString() => Name; // Qo‘shildi
}
