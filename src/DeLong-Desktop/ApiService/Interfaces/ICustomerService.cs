using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.DTOs.Customers;

namespace DeLong_Desktop.ApiService.Interfaces;

interface ICustomerService
{
    ValueTask<bool> AddAsync(CustomerCreationDto dto);
    ValueTask<bool> ModifyAsync(CustomerUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<CustomerResultDto> RetrieveByIdAsync(long id);
    ValueTask<CustomerResultDto> RetrieveByInnAsync(int INN);
    ValueTask<IEnumerable<CustomerResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null);
    ValueTask<IEnumerable<CustomerResultDto>> RetrieveAllAsync();
}
