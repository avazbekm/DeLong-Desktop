using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.Warehouses;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IWarehouseService
{
    ValueTask<bool> AddAsync(WarehouseCreationDto dto);
    ValueTask<bool> ModifyAsync(WarehouseUpdatedDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<WarehouseResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<WarehouseResultDto>> RetrieveAllAsync();
}
