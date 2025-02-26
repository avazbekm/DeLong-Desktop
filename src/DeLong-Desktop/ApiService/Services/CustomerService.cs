using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.Customers;
using DeLong_Desktop.ApiService.Configurations;

namespace DeLong_Desktop.ApiService.Services;

class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    public CustomerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async ValueTask<bool> AddAsync(CustomerCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Customer/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(CustomerUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Customer/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Customer/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<CustomerResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Customer/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<CustomerResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<CustomerResultDto> RetrieveByInnAsync(int INN)
    {
        var response = await _httpClient.GetAsync($"api/Customer/get/INN?inn={INN}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<CustomerResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<CustomerResultDto> RetrieveByJshshirAsync(string jshshir)
    {
        var response = await _httpClient.GetAsync($"api/Customer/get/Jshshir?jshshir={jshshir}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<CustomerResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<CustomerResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null)
    {
        var response = await _httpClient.GetAsync("api/Customer/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<CustomerResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<CustomerResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Customer/get-allCustomers");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<CustomerResultDto>>>(jsonResponse);
        return result.Data;
    }

}
