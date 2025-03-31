using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Branchs;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class BranchService : IBranchService
{
    private readonly HttpClient _httpClient;

    public BranchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<BranchResultDto> AddAsync(BranchCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Branch/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<BranchResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<BranchResultDto> ModifyAsync(BranchUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/Branch/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<BranchResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Branch/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<BranchResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Branch/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<BranchResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<BranchResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Branch/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<BranchResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<BranchResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<BranchResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<BranchResultDto>();
        }
    }
}