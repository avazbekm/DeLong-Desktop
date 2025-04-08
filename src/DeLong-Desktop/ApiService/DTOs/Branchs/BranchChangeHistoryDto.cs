namespace DeLong_Desktop.ApiService.DTOs.Branchs;

public class BranchChangeHistoryDto
{
    public long Id { get; set; } // ChangeHistory ID’si
    public string ChangeType { get; set; } = string.Empty; // Insert, Update, Delete
    public string? OldValues { get; set; } // JSON sifatida eski qiymatlar
    public string? NewValues { get; set; } // JSON sifatida yangi qiymatlar
    public DateTimeOffset CreatedAt { get; set; } // O‘zgarish vaqti
    public long? CreatedBy { get; set; } // Kim o‘zgartirgan
}