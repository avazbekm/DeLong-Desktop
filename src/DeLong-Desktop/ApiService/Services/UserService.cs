﻿using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.DTOs.Users;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.Configurations;
using System.Net.Http.Json;

namespace DeLong_Desktop.ApiService.Services;

class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    public UserService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }

     public async ValueTask<bool> AddAsync(UserCreationDto user)
    {
        var jsonContent = JsonConvert.SerializeObject(user);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/User/create", content);
        response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {response.StatusCode}, Details: {error}");
        }

        return response.IsSuccessStatusCode;
        //return (await response.Content.ReadFromJsonAsync<Response<UserResultDto>>())!;
    }

    public async ValueTask<bool> ModifyAsync(UserUpdateDto user)
    {
        var jsonContent = JsonConvert.SerializeObject(user);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/User/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/User/delete/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<UserResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/User/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<UserResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<UserResultDto> RetrieveByJSHSHIRAsync(long Jshshir)
    {
        var response = await _httpClient.GetAsync($"api/User/get/{Jshshir}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<UserResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null)
    {
        var response = await _httpClient.GetAsync("api/User/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<UserResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<UserResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/User/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<UserResultDto>>>(jsonResponse);
        return result.Data;
    }
}
