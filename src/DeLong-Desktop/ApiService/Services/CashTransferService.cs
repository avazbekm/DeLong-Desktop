using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.CashTransfers;


namespace DeLong_Desktop.ApiService.Services;

public class CashTransferService : ICashTransferService
{
    private readonly HttpClient _httpClient;

    public CashTransferService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<CashTransferResultDto> AddAsync(CashTransferCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CashTransfer/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashTransferResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<CashTransferResultDto> ModifyAsync(CashTransferUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/CashTransfer/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashTransferResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/CashTransfer/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<CashTransferResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashTransfer/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CashTransferResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<CashTransferResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/CashTransfer/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashTransferResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashTransferResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashTransferResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashTransferResultDto>();
        }
    }

    public async ValueTask<IEnumerable<CashTransferResultDto>> RetrieveAllByCashRegisterIdAsync(long cashRegisterId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CashTransfer/get-by-cashregister/{cashRegisterId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CashTransferResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CashTransferResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CashTransferResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return Enumerable.Empty<CashTransferResultDto>();
        }
    }
}