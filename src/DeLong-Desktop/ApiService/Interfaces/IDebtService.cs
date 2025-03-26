using DeLong_Desktop.ApiService.DTOs.Debts;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IDebtService
{
    ValueTask<DebtResultDto> AddAsync(DebtCreationDto dto);
    ValueTask<DebtResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<DebtResultDto>> RetrieveAllAsync();
    ValueTask<IEnumerable<DebtResultDto>> RetrieveBySaleIdAsync(long saleId);
    ValueTask<Dictionary<string, List<DebtResultDto>>> RetrieveAllGroupedByCustomerAsync(); // Yangi metod
}