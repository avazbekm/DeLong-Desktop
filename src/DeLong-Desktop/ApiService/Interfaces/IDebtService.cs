using DeLong_Desktop.ApiService.DTOs.Debts;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IDebtService
{
    ValueTask<DebtResultDto> AddAsync(DebtCreationDto dto);
    ValueTask<DebtResultDto> ModifyAsync(DebtUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<DebtResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<DebtResultDto>> RetrieveAllAsync();
}