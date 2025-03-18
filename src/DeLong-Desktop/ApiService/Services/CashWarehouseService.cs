using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.CashWarehouses;


namespace DeLong_Desktop.ApiService.Services;

public class CashWarehouseService : ICashWarehouseService
{
    private readonly HttpClient _httpClient;

    public CashWarehouseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<CashWarehouseResultDto> AddAsync(CashWarehouseCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CashWarehouse/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashWarehouseResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<CashWarehouseResultDto> ModifyAsync(CashWarehouseUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/CashWarehouse/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashWarehouseResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/CashWarehouse/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<CashWarehouseResultDto> RetrieveByIdAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashWarehouse/get");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashWarehouseResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<CashWarehouseResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/CashWarehouse/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashWarehouseResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashWarehouseResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashWarehouseResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashWarehouseResultDto>();
        }
    }
}