using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.ApiService.DTOs.Transactions;

public class TransactionCreationDto
{
    public long? SupplierIdFrom { get; set; } // Yetkazib beruvchi ID (ixtiyoriy)
    public long BranchId { get; set; } // Tranzaksiya manba filiali ID
    public long? BranchIdTo { get; set; } // Tranzaksiya qabul qiluvchi filial ID (ixtiyoriy)
    public TransactionType TransactionType { get; set; } // Tranzaksiya turi (kirim, chiqim, qaytarish)
    public string Comment { get; set; } = string.Empty; // Izoh (bo‘sh bo‘lishi mumkin)
    public Guid? RequestId { get; set; } // Idempotentlik uchun qo‘shildi
    public List<TransactionItemCreationDto> Items { get; set; } = new List<TransactionItemCreationDto>(); // Tranzaksiya elementlari
}