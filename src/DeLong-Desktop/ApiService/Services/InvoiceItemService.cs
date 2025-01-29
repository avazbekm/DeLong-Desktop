using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.InvoiceItems;

namespace DeLong_Desktop.ApiService.Services;

class InvoiceItemService : IInvoiceItemService
{
    private readonly HttpClient _httpClient;
    public InvoiceItemService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }

    public async ValueTask<bool> AddAsync(InvoiceItemCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/InvoiceItem/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(InvoiceItemUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/InvoiceItem/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/InvoiceItem/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<InvoiceItemResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/InvoiceItem/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<InvoiceItemResultDto>>(jsonResponse);
        return result.Data;
    }
    
    public async ValueTask<IEnumerable<InvoiceItemResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/InvoiceItem/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<InvoiceItemResultDto>>>(jsonResponse);
        return result.Data;
    }
}
