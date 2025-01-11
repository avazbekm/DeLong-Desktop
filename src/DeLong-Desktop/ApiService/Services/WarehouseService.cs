using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.Warehouses;

namespace DeLong_Desktop.ApiService.Services;

class WarehouseService : IWarehouseService
{
    private readonly HttpClient _httpClient;

    public WarehouseService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7049/") // API URL manzilini o'rnating
        };
    }

    public async ValueTask<bool> AddAsync(WarehouseCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Warehouse/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(WarehouseUpdatedDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Warehouse/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Warehouse/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<WarehouseResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Warehouse/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<WarehouseResultDto>>(jsonResponse);
        return result.Data;
    }
    
    public async ValueTask<IEnumerable<WarehouseResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("/api/Warehouse/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<WarehouseResultDto>>>(jsonResponse);
        return result.Data;
    }
}
