using DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface ICreditorDebtPaymentService
{
    ValueTask<CreditorDebtPaymentResultDto> AddAsync(CreditorDebtPaymentCreationDto dto);
    ValueTask<CreditorDebtPaymentResultDto> ModifyAsync(CreditorDebtPaymentUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<CreditorDebtPaymentResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<CreditorDebtPaymentResultDto>> RetrieveAllAsync();
}