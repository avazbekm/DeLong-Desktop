using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

class UserService
{
    private readonly HttpClient _httpClient;

    public UserService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://yourapiurl.com/") // API URL manzilini o'rnating
        };
    }

    public async Task<List<UserResultDto>> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync("api/users/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<UserResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async Task<UserResultDto> GetUserByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/users/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<UserResultDto>>(jsonResponse);
        return result.Data;
    }

    public async Task<bool> AddUserAsync(UserCreationDto user)
    {
        var jsonContent = JsonConvert.SerializeObject(user);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/users/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserAsync(UserUpdateDto user)
    {
        var jsonContent = JsonConvert.SerializeObject(user);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/users/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/users/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }
}
