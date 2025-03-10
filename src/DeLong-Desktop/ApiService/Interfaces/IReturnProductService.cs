using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IReturnProductService
{
    ValueTask<ReturnProductResultDto> AddAsync(ReturnProductCreationDto dto);
    ValueTask<ReturnProductResultDto> ModifyAsync(ReturnProductUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<ReturnProductResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<ReturnProductResultDto>> RetrieveBySaleIdAsync(long saleId);
    ValueTask<IEnumerable<ReturnProductResultDto>> RetrieveByProductIdAsync(long productId);
}