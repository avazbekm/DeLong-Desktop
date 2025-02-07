using DeLong_Desktop.ApiService.DTOs.KursDollar;

namespace DeLong_Desktop.ApiService.Interfaces;

public interface IKursDollarService
{
    ValueTask<bool> AddAsync(KursDollarCreationDto dto);
    ValueTask<KursDollarResultDto> RetrieveByIdAsync();
    ValueTask<IEnumerable<KursDollarResultDto>> RetrieveAllAsync();
}
