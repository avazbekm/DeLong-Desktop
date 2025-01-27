using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Invoices;

namespace DeLong_Desktop.ApiService.Services;

class InvoiceService : IInvoiceService
{
    public ValueTask<bool> AddAsync(InvoiceCreationDto dto)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> ModifyAsync(InvoiceUpdateDto dto)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IEnumerable<InvoiceResultDto>> RetrieveAllAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask<InvoiceResultDto> RetrieveByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}
