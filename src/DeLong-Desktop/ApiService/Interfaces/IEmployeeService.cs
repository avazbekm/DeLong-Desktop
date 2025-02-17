using DeLong_Desktop.ApiService.DTOs.Employees;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IEmployeeService
{
    ValueTask<EmployeeResultDto> AddAsync(EmployeeCreationDto dto);
    ValueTask<EmployeeResultDto> ModifyAsync(EmployeeUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<EmployeeResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<EmployeeResultDto>> RetrieveAllAsync();
}