using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using DeLong_Desktop.ApiService.Interfaces;
using DeLong_Desktop.ApiService.DTOs.Products;
using DeLong_Desktop.ApiService.Configurations;
using DeLong_Desktop.ApiService.Models.Commons;
using System.Windows;

namespace DeLong_Desktop.ApiService.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask<bool> AddAsync(ProductCreationDto dto)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Product/create", content);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<bool> ModifyAsync(ProductUpdateDto dto)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/Product/update", content);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<bool> RemoveAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Product/delete/{id}");
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return false;
        }
    }

    public async ValueTask<ProductResultDto> RetrieveByIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Product/get/{id}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<ProductResultDto>>(jsonResponse);
            return result.Data;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<ProductResultDto>> RetrieveAllAsync(PaginationParams @params, Filter filter, string search = null)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Product/get-all");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<ProductResultDto>>>(jsonResponse);
            return result.Data;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }

    public async ValueTask<IEnumerable<ProductResultDto>> RetrieveAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Product/get-allProducts");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<List<ProductResultDto>>>(jsonResponse);
            return result.Data;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            MessageBox.Show("Sessiya tugadi, qayta login qiling!");
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
            return null;
        }
    }
}