using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.DTOs.Transactions;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ITransactionProcessingService
{
    Task<TransactionResultDto> ProcessTransactionAsync(List<ReceiveItem> receiveItems);
}