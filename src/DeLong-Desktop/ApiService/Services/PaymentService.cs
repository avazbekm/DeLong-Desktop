using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Payments;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient) // Dependency Injection orqali HttpClient olindi
    {
        _httpClient = httpClient;
    }

    public async ValueTask<PaymentResultDto> AddAsync(PaymentCreationDto dto)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Payment/create", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<PaymentResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<PaymentResultDto>> RetrieveBySaleIdAsync(long saleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Payment/get-by-sale/{saleId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<PaymentResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<PaymentResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<PaymentResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<PaymentResultDto>();
        }
    }

    public async ValueTask<IEnumerable<PaymentResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Payment/get-all");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<PaymentResultDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<PaymentResultDto>>>(jsonResponse);
            return result?.Data ?? Enumerable.Empty<PaymentResultDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            return Enumerable.Empty<PaymentResultDto>();
        }
    }
}