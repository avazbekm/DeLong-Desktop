using System.Text;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;

namespace DeLong_Desktop.ApiService.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<string> LoginAsync(string username, string password)
    {
        try
        {
            var loginData = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<Response<string>>(errorJson);
                MessageBox.Show($"Xatolik: {errorResult?.Message ?? "Server javob bermadi"}");
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<string>>(jsonResponse);
            return result?.Data; // Access token qaytadi
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }
}