using DeLong_Desktop.ApiService.DTOs.CashWarehouses;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ICashWarehouseService
{
    ValueTask<CashWarehouseResultDto> AddAsync(CashWarehouseCreationDto dto);
    ValueTask<CashWarehouseResultDto> ModifyAsync(CashWarehouseUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<CashWarehouseResultDto> RetrieveByIdAsync();
    ValueTask<IEnumerable<CashWarehouseResultDto>> RetrieveAllAsync();
}