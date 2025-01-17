using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Category;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    public CategoryService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }
    public async ValueTask<bool> AddAsync(CategoryCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Category/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> ModifyAsync(CategoryUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Category/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Category/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }
    public async ValueTask<CategoryResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Category/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<CategoryResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<CategoryResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Category/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<CategoryResultDto>>>(jsonResponse);
        return result.Data;
    }

}
