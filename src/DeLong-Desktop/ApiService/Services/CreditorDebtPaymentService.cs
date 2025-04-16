using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons; // MessageBox uchun
using DeLong_Desktop.ApiService.DTOs.CreditorDebtPayments;

namespace DeLong_Desktop.ApiService.Services;

public class CreditorDebtPaymentService : ICreditorDebtPaymentService
{
    private readonly HttpClient _httpClient;

    public CreditorDebtPaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<CreditorDebtPaymentResultDto> AddAsync(CreditorDebtPaymentCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CreditorDebtPayment/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtPaymentResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<CreditorDebtPaymentResultDto> ModifyAsync(CreditorDebtPaymentUpdateDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/CreditorDebtPayment/update", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtPaymentResultDto>>(jsonResponse);
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
            var response = await _httpClient.DeleteAsync($"api/CreditorDebtPayment/remove/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public async ValueTask<CreditorDebtPaymentResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/CreditorDebtPayment/get/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<CreditorDebtPaymentResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async ValueTask<IEnumerable<CreditorDebtPaymentResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/CreditorDebtPayment/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<CreditorDebtPaymentResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<CreditorDebtPaymentResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<CreditorDebtPaymentResultDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return Enumerable.Empty<CreditorDebtPaymentResultDto>();
        }
    }
}