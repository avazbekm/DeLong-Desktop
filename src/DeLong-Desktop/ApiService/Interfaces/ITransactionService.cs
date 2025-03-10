using DeLong_Desktop.ApiService.DTOs.Transactions;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ITransactionService
{
    ValueTask<TransactionResultDto> AddAsync(TransactionCreationDto dto);
    ValueTask<TransactionResultDto> ModifyAsync(TransactionUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<TransactionResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<TransactionResultDto>> RetrieveAllAsync();
}