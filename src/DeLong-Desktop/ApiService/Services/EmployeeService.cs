using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.Models.Commons;
using DeLong_Desktop.ApiService.DTOs.Employees;

namespace DeLong_Desktop.ApiService.Services;

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _httpClient;
    public EmployeeService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5208/") // API URL manzilini o'rnating
        };
    }

    public async ValueTask<EmployeeResultDto> AddAsync(EmployeeCreationDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Employee/create", content);
        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Response<EmployeeResultDto>>(responseString);
        return result.Data; // Foydalanuvchi qo'shildi, uning ma'lumotlari qaytariladi ✅
    }

    public async ValueTask<EmployeeResultDto> ModifyAsync(EmployeeUpdateDto dto)
    {
        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("api/Employee/update", content);
        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Response<EmployeeResultDto>>(responseString);
        return result.Data; // Yangilangan xodim ma'lumotlari qaytariladi ✅
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/Employee/delete/{id}");

        return response.IsSuccessStatusCode; // Agar muvaffaqiyatli bo'lsa, true qaytaradi ✅
    }

    public async ValueTask<EmployeeResultDto> RetrieveByIdAsync(long id)
    {
        var response = await _httpClient.GetAsync($"api/Employee/get/{id}");
        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Response<EmployeeResultDto>>(responseString);
        return result.Data; // ID bo‘yicha xodim ma’lumotini qaytaradi ✅
    }

    public async ValueTask<IEnumerable<EmployeeResultDto>> RetrieveAllAsync()
    {
        var response = await _httpClient.GetAsync("api/Employee/get-all");
        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Response<IEnumerable<EmployeeResultDto>>>(responseString);
        return result.Data; // Barcha xodimlar ro‘yxatini qaytaradi ✅
    }
}