using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.SaleItems;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class SaleItemService : ISaleItemService
{
    private readonly HttpClient _httpClient;

    public SaleItemService(HttpClient httpClient) // Dependency Injection orqali HttpClient olindi
    {
        _httpClient = httpClient;
    }

    public async ValueTask<SaleItemResultDto> AddAsync(SaleItemCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/SaleItem/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SaleItemResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<SaleItemResultDto> ModifyAsync(SaleItemUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/SaleItem/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SaleItemResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/SaleItem/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<SaleItemResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/SaleItem/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SaleItemResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<SaleItemResultDto>> RetrieveAllBySaleIdAsync(long saleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/SaleItem/get-by-sale/{saleId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<SaleItemResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<SaleItemResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<SaleItemResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<SaleItemResultDto>();
        }
    }
}
