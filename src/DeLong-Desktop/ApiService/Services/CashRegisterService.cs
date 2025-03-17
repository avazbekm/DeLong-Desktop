using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.CashRegisters;


namespace DeLong_Desktop.ApiService.Services;

public class CashRegisterService : ICashRegisterService
{
    private readonly HttpClient _httpClient;

    public CashRegisterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<CashRegisterResultDto> AddAsync(CashRegisterCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CashRegister/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashRegisterResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<CashRegisterResultDto> ModifyAsync(CashRegisterUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/CashRegister/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashRegisterResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/CashRegister/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<CashRegisterResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashRegister/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashRegisterResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<CashRegisterResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/CashRegister/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashRegisterResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashRegisterResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashRegisterResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashRegisterResultDto>();
        }
    }

    public async ValueTask<IEnumerable<CashRegisterResultDto>> RetrieveAllByUserIdAsync(long userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashRegister/get-by-user/{userId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashRegisterResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashRegisterResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashRegisterResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashRegisterResultDto>();
        }
    }

    public async ValueTask<IEnumerable<CashRegisterResultDto>> RetrieveAllByWarehouseIdAsync(long warehouseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashRegister/get-by-warehouse/{warehouseId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashRegisterResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashRegisterResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashRegisterResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashRegisterResultDto>();
        }
    }
}