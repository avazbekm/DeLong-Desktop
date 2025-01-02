using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.Category;

namespace DeLong_Desktop.ApiService.Interfaces;

interface ICategoryService
{
    ValueTask<CategoryResultDto> AddAsync(CategoryCreationDto dto);
    ValueTask<CategoryResultDto> ModifyAsync(CategoryUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<CategoryResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<CategoryResultDto>> RetrieveAllAsync();
}
