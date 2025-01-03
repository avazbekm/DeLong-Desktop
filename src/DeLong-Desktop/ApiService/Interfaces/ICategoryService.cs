using DeLong_Desktop.ApiService.DTOs.Category;

namespace DeLong_Desktop.ApiService.Interfaces;

interface ICategoryService
{
    ValueTask<bool> AddAsync(CategoryCreationDto dto);
    ValueTask<bool> ModifyAsync(CategoryUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<CategoryResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<CategoryResultDto>> RetrieveAllAsync();
}
