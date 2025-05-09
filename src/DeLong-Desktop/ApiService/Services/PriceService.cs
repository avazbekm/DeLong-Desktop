﻿using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Prices;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

class PriceService : IPriceService
{
    private readonly HttpClient _httpClient;
    public PriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<PriceResultDto> AddAsync(PriceCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Price/create", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PriceResultDto>(responseContent);

        return result!;
    }


    public async ValueTask<bool> ModifyAsync(PriceUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Price/update", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Price/remove/{id}");
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<PriceResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Price/get/{id}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<PriceResultDto>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<PriceResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Price/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<PriceResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<IEnumerable<PriceResultDto>> RetrieveAllAsync(long productId)
    {
        var response = await _httpClient.GetAsync($"api/Price/get-all-product/{productId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<PriceResultDto>>>(jsonResponse);
        return result.Data;
    }
}
