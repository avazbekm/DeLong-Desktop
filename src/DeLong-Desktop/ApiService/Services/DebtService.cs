using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Debts;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class DebtService : IDebtService
{
    private readonly HttpClient _httpClient;

    public DebtService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<DebtResultDto> AddAsync(DebtCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Debt/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<DebtResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<DebtResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Debt/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<DebtResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<DebtResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Debt/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DebtResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<DebtResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<DebtResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<DebtResultDto>();
        }
    }

    public async ValueTask<IEnumerable<DebtResultDto>> RetrieveBySaleIdAsync(long saleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Debt/get-by-sale/{saleId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DebtResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<DebtResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<DebtResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<DebtResultDto>();
        }
    }

    public async ValueTask<Dictionary<string, List<DebtResultDto>>> RetrieveAllGroupedByCustomerAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Debt/get-all-grouped-by-customer");

            if (!response.IsSuccessStatusCode)
                return new Dictionary<string, List<DebtResultDto>>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<Dictionary<string, List<DebtResultDto>>>>(jsonResponse);
            return result?.Data ?? new Dictionary<string, List<DebtResultDto>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return new Dictionary<string, List<DebtResultDto>>();
        }
    }
}