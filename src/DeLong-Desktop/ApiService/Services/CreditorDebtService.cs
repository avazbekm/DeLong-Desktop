using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons; // MessageBox uchun
using DeLong_Desktop.ApiService.DTOs.CreditorDebts;


namespace DeLong_Desktop.ApiService.Services;

public class CreditorDebtService : ICreditorDebtService
{
    private readonly HttpClient _httpClient;

    public CreditorDebtService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<CreditorDebtResultDto> AddAsync(CreditorDebtCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CreditorDebt/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<CreditorDebtResultDto> ModifyAsync(CreditorDebtUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/CreditorDebt/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/CreditorDebt/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public async ValueTask<CreditorDebtResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CreditorDebt/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<IEnumerable<CreditorDebtResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/CreditorDebt/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CreditorDebtResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CreditorDebtResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CreditorDebtResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return Enumerable.Empty<CreditorDebtResultDto>();
        }
    }
}