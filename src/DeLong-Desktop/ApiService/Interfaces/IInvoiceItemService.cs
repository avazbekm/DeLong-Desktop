using DeLong_Desktop.ApiService.DTOs.InvoiceItems;

namespace DeLong_Desktop.ApiService.Interfaces;

interface IInvoiceItemService
{
    ValueTask<bool> AddAsync(InvoiceItemCreationDto dto);
    ValueTask<bool> ModifyAsync(InvoiceItemUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<InvoiceItemResultDto> RetrieveByIdAsync(long id);
    ValueTask<IEnumerable<InvoiceItemResultDto>> RetrieveAllAsync();
}