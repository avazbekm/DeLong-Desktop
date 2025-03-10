using DeLong_Desktop.ApiService.DTOs.TransactionItems;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ITransactionItemService
{
    /// <summary>
    /// Yangi transaction item qo'shadi
    /// </summary>
    /// <param name="dto">TransactionItemCreationDto obyekt</param>
    /// <returns>Yaratilgan transaction item</returns>
    ValueTask<TransactionItemResultDto> AddAsync(TransactionItemCreationDto dto);

    /// <summary>
    /// Transaction item ni yangilaydi
    /// </summary>
    /// <param name="dto">TransactionItemUpdateDto obyekt</param>
    /// <returns>Yangilangan transaction item</returns>
    ValueTask<TransactionItemResultDto> ModifyAsync(TransactionItemUpdateDto dto);

    /// <summary>
    /// Transaction item ni o'chiradi
    /// </summary>
    /// <param name="id">Transaction item ID si</param>
    /// <returns>O'chirish natijasi</returns>
    ValueTask<bool> RemoveAsync(long id);

    /// <summary>
    /// Transaction item ni ID bo'yicha qidiradi
    /// </summary>
    /// <param name="id">Transaction item ID si</param>
    /// <returns>Topilgan transaction item</returns>
    ValueTask<TransactionItemResultDto> RetrieveByIdAsync(long id);

    /// <summary>
    /// Barcha transaction item larni qaytaradi
    /// </summary>
    /// <returns>Transaction item lar ro'yxati</returns>
    ValueTask<IEnumerable<TransactionItemResultDto>> RetrieveAllAsync();
}