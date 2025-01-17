using DeLong_Desktop.ApiService.DTOs.Prices;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IPriceService
{
    ValueTask<bool> AddAsync(PriceCreationDto dto);
    ValueTask<bool> ModifyAsync(PriceUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<PriceResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<PriceResultDto>> RetrieveAllAsync();
    ValueTask<IEnumerable<PriceResultDto>> RetrieveAllAsync(long productId);

}
