using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.KursDollar;

namespace DeLong_Desktop.ApiService.Services;

public class KursDollarService : IKursDollarService
{
    private readonly HttpClient _httpClient;

    public KursDollarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<bool> AddAsync(KursDollarCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/KursDollar/create", content);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<IEnumerable<KursDollarResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("/api/KursDollar/get-all");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<List<KursDollarResultDto>>>(jsonResponse);
        return result.Data;
    }

    public async ValueTask<KursDollarResultDto> RetrieveByIdAsync()
    {
        var response = await _httpClient.GetAsync($"api/KursDollar/get");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response<KursDollarResultDto>>(jsonResponse);
        return result.Data;
    }
}
