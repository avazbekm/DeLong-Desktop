using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.ApiService.DTOs.Transactions;

public class TransactionUpdateDto
{
    public long Id { get; set; } // Yangilanadigan transaksiyaning identifikatori
    public long? WarehouseIdTo { get; set; }   // Yangilanishi mumkin bo‘lgan kirim ombori
    public List<TransactionItemUpdateDto> Items { get; set; } = new List<TransactionItemUpdateDto>(); // Mahsulotlar ro‘yxati
    public TransactionType? TransactionType { get; set; } // Yangilanishi mumkin bo‘lgan turi
    public string Comment { get; set; } = string.Empty;   // Yangilanishi mumkin
}
