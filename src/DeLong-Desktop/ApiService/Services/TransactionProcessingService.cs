using System.Text;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.Transactions;


public class TransactionProcessingService : ITransactionProcessingService
{
    private readonly HttpClient _httpClient;

    public TransactionProcessingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TransactionResultDto> ProcessTransactionAsync(List<ReceiveItem> receiveItems)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(receiveItems), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/TransactionProcessing/process", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Tranzaksiyani yuborishda xato: {errorMessage}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<TransactionResultDto>>(jsonResponse);
            return result?.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }
}