using DeLong_Desktop.ApiService.DTOs.Branchs;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IBranchService
{
    /// <summary>
    /// Yangi filial qo‘shadi.
    /// </summary>
    /// <param name="dto">Filial yaratish uchun ma’lumotlar</param>
    /// <returns>Yaratilgan filialning ID’si</returns>
    ValueTask<BranchResultDto> AddAsync(BranchCreationDto dto);

    /// <summary>
    /// Barcha filiallar ro‘yxatini qaytaradi.
    /// </summary>
    /// <returns>Filiallar ro‘yxati</returns>
    ValueTask<IEnumerable<BranchResultDto>> RetrieveAllAsync();

    /// <summary>
    /// Berilgan ID bo‘yicha filialni qaytaradi.
    /// </summary>
    /// <param name="id">Filial ID’si</param>
    /// <returns>Filial ma’lumotlari yoki null</returns>
    ValueTask<BranchResultDto> RetrieveByIdAsync(long id);

    /// <summary>
    /// Filial ma’lumotlarini yangilaydi.
    /// </summary>
    /// <param name="dto">Yangilangan filial ma’lumotlari</param>
    /// <returns>Yangilash muvaffaqiyatli bo‘lsa true</returns>
    ValueTask<BranchResultDto> ModifyAsync(BranchUpdateDto dto);

    /// <summary>
    /// Filialni o‘chiradi (soft delete).
    /// </summary>
    /// <param name="id">O‘chiriladigan filial ID’si</param>
    /// <returns>O‘chirish muvaffaqiyatli bo‘lsa true</returns>
    ValueTask<bool> RemoveAsync(long id);
}