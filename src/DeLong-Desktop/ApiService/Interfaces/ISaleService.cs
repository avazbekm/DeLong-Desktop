using DeLong_Desktop.ApiService.DTOs.Sales;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ISaleService
{
    ValueTask<SaleResultDto> AddAsync(SaleCreationDto dto);
    ValueTask<SaleResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<SaleResultDto>> RetrieveAllAsync();
}