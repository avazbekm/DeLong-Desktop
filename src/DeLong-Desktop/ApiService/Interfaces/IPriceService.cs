using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.Prices;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IPriceService
{
    ValueTask<PriceResultDto> AddAsync(PriceCreationDto dto);
    ValueTask<PriceResultDto> ModifyAsync(PriceUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<PriceResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<PriceResultDto>> RetrieveAllAsync();
}
