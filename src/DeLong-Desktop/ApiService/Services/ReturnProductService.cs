using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.DTOs;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class ReturnProductService : IReturnProductService
{
    private readonly HttpClient _httpClient;

    public ReturnProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<ReturnProductResultDto> AddAsync(ReturnProductCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/ReturnProduct/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<ReturnProductResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<ReturnProductResultDto> ModifyAsync(ReturnProductUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/ReturnProduct/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<ReturnProductResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/ReturnProduct/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<ReturnProductResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/ReturnProduct/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<ReturnProductResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<ReturnProductResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ReturnProduct/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ReturnProductResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<ReturnProductResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<ReturnProductResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<ReturnProductResultDto>();
        }
    }

    public async ValueTask<IEnumerable<ReturnProductResultDto>> RetrieveBySaleIdAsync(long saleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/ReturnProduct/get-by-sale/{saleId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ReturnProductResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<ReturnProductResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<ReturnProductResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<ReturnProductResultDto>();
        }
    }

    public async ValueTask<IEnumerable<ReturnProductResultDto>> RetrieveByProductIdAsync(long productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/ReturnProduct/get-by-product/{productId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ReturnProductResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<ReturnProductResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<ReturnProductResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<ReturnProductResultDto>();
        }
    }
}