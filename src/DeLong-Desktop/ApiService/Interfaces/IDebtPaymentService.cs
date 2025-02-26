using DeLong_Desktop.ApiService.DTOs.DebtPayments;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IDebtPaymentService
{
    ValueTask<DebtPaymentResultDto> AddAsync(DebtPaymentCreationDto dto);
    ValueTask<IEnumerable<DebtPaymentResultDto>> RetrieveByDebtIdAsync(long debtId);
    ValueTask<IEnumerable<DebtPaymentResultDto>> RetrieveAllAsync();
}