using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    public ProductService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }

    public async ValueTask<bool> AddAsync(ProductCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Product/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(ProductUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Product/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Product/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }
    public async ValueTask<ProductResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"/api/Product/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<ProductResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<ProductResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null)
    {
        var response = await _httpClient.GetAsync("api/Product/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<ProductResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<ProductResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Product/get-allProducts");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<ProductResultDto>>>(jsonResponse);
        return result.Data;
    }

}
