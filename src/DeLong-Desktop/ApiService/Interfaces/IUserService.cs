using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Configurations;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IUserService
{
    ValueTask<bool> AddAsync(UserCreationDto dto);
    ValueTask<bool> ModifyAsync(UserUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<UserResultDto> RetrieveByIdAsync(long id);
    ValueTask<UserResultDto> RetrieveByJSHSHIRAsync(long Jshshir);
    ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null);
    ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync();
}
