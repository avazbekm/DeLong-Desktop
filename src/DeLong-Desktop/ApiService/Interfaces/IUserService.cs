using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.Users;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IUserService
{
    ValueTask<bool> AddAsync(UserCreationDto dto);
    ValueTask<bool> ModifyAsync(UserUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<UserResultDto> RetrieveByIdAsync(long id);
    ValueTask<UserResultDto> RetrieveByJSHSHIRAsync(long Jshshir);
    ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null);
    ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync();
}
