using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.ApiService.DTOs.Transactions;

public class TransactionUpdateDto
{
    public long Id { get; set; } // Yangilanadigan tranzaksiya ID
    public long? SupplierIdFrom { get; set; } // Yetkazib beruvchi ID (ixtiyoriy)
    public long BranchId { get; set; } // Manba filial ID
    public long? BranchIdTo { get; set; } // Qabul qiluvchi filial ID (ixtiyoriy)
    public TransactionType TransactionType { get; set; } // Tranzaksiya turi
    public string Comment { get; set; } = string.Empty; // Izoh
    public List<TransactionItemUpdateDto> Items { get; set; } = new List<TransactionItemUpdateDto>(); // Yangilanadigan elementlar
}
