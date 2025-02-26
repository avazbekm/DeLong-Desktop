using DeLong_Desktop.ApiService.DTOs.Discounts;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IDiscountService
{
    ValueTask<DiscountResultDto> AddAsync(DiscountCreationDto dto);
    ValueTask<DiscountResultDto> ModifyAsync(DiscountUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<DiscountResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<DiscountResultDto>> RetrieveAllAsync();
}
