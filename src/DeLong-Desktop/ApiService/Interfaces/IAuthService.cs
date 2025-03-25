namespace DeLong_Desktop.ApiService.Interfaces;

public interface IAuthService
{
    ValueTask<string> LoginAsync(string username, string password);
}
