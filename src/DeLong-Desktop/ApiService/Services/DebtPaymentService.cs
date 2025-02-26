using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.DebtPayments;

namespace DeLong_Desktop.ApiService.Services;

public class DebtPaymentService : IDebtPaymentService
{
    private readonly HttpClient _httpClient;

    public DebtPaymentService(HttpClient httpClient) // Dependency Injection orqali HttpClient olindi
    {
        _httpClient = httpClient;
    }

    public async ValueTask<DebtPaymentResultDto> AddAsync(DebtPaymentCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/DebtPayment/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<DebtPaymentResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<DebtPaymentResultDto>> RetrieveByDebtIdAsync(long debtId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/DebtPayment/get-by-debt/{debtId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DebtPaymentResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<DebtPaymentResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<DebtPaymentResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<DebtPaymentResultDto>();
        }
    }

    public async ValueTask<IEnumerable<DebtPaymentResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/DebtPayment/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<DebtPaymentResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<DebtPaymentResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<DebtPaymentResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<DebtPaymentResultDto>();
        }
    }
}
