using DeLong_Desktop.ApiService.DTOs.Payments;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IPaymentService
{
    ValueTask<PaymentResultDto> AddAsync(PaymentCreationDto dto);
    ValueTask<IEnumerable<PaymentResultDto>> RetrieveBySaleIdAsync(long saleId);
    ValueTask<IEnumerable<PaymentResultDto>> RetrieveAllAsync();
}