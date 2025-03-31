using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.Suppliers;

namespace DeLong_Desktop.ApiService.Services;


public class SupplierService : ISupplierService
{
    private readonly HttpClient _httpClient;

    public SupplierService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<SupplierResultDto> AddAsync(SupplierCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Supplier/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SupplierResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<SupplierResultDto> ModifyAsync(SupplierUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/Supplier/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SupplierResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/Supplier/delete/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public async ValueTask<SupplierResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Supplier/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<SupplierResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<IEnumerable<SupplierResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Supplier/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<SupplierResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<SupplierResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<SupplierResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return Enumerable.Empty<SupplierResultDto>();
        }
    }
}