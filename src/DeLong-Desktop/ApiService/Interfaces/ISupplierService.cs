using DeLong_Desktop.ApiService.DTOs.Suppliers;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ISupplierService
{
    ValueTask<SupplierResultDto> AddAsync(SupplierCreationDto dto);
    ValueTask<SupplierResultDto> ModifyAsync(SupplierUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<SupplierResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<SupplierResultDto>> RetrieveAllAsync();
}