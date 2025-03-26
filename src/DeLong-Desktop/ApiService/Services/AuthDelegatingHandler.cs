using System.Net.Http;
using System.Net.Http.Headers;

namespace DeLong_Desktop.ApiService.Services;

public class AuthDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = App.Current.Properties["Token"]?.ToString();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}