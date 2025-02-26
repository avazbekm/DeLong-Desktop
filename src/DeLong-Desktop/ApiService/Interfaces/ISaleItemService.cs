using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.SaleItems;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ISaleItemService
{
    ValueTask<SaleItemResultDto> AddAsync(SaleItemCreationDto dto);
    ValueTask<SaleItemResultDto> ModifyAsync(SaleItemUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<SaleItemResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<SaleItemResultDto>> RetrieveAllBySaleIdAsync(long saleId);
}