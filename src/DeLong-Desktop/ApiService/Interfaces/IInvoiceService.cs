using DeLong_Desktop.ApiService.DTOs.Invoices;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IInvoiceService
{
    ValueTask<bool> AddAsync(InvoiceCreationDto dto);
    ValueTask<bool> ModifyAsync(InvoiceUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<InvoiceResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<InvoiceResultDto>> RetrieveAllAsync();
}
