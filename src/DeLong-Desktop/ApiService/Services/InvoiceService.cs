using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Invoices;
using Newtonsoft.Json;
using System.Text;
using DeLong_Desktop.ApiService.DTOs.Customers;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

class InvoiceService : IInvoiceService
{
    private readonly HttpClient _httpClient;
    public InvoiceService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }

    public async ValueTask<bool> AddAsync(InvoiceCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Invoice/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(InvoiceUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Invoice/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Invoice/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }
 
    public async ValueTask<InvoiceResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Invoice/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<InvoiceResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<InvoiceResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Invoice/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<InvoiceResultDto>>>(jsonResponse);
        return result.Data;
    }
}
