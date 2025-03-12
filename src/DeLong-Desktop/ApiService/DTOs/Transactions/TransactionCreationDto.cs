using DeLong_Desktop.ApiService.DTOs.Enums;
using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.ApiService.DTOs.Transactions;

public class TransactionCreationDto
{
    public long WarehouseIdTo { get; set; }   // Kirim ombori
    public List<TransactionItemCreationDto> Items { get; set; } = new List<TransactionItemCreationDto>(); // Mahsulotlar ro‘yxati
    public TransactionType TransactionType { get; set; }
    public string Comment { get; set; } = string.Empty;
}   