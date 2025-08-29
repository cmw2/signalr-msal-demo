using Microsoft.Identity.Web;

namespace WebApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ApiService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<string> CallAPINoGraphAsync()
    {
        try
        {
            var baseUrl = _configuration["APIEndpoints:APINoGraphBaseUrl"];
            var scopes = _configuration.GetSection("DownstreamApis:APINoGraph:Scopes").Get<string[]>();
            
            if (string.IsNullOrEmpty(baseUrl) || scopes == null || scopes.Length == 0)
            {
                return "Configuration missing for APINoGraph";
            }
            
            var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scopes[0]);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await _httpClient.GetAsync($"{baseUrl}/api/hello");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            
            return $"Error: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    public async Task<string> CallAPIWithGraphAsync(Func<Task>? onMsalUiRequired = null)
    {
        try
        {
            var baseUrl = _configuration["APIEndpoints:APIWithGraphBaseUrl"];
            var scopes = _configuration.GetSection("DownstreamApis:APIWithGraph:Scopes").Get<string[]>();
            
            if (string.IsNullOrEmpty(baseUrl) || scopes == null || scopes.Length == 0)
            {
                return "Configuration missing for APIWithGraph";
            }
            
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await _httpClient.GetAsync($"{baseUrl}/api/profile/mobile-greeting");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            
            return $"Error: {response.StatusCode}";
        }
        catch (Microsoft.Identity.Web.MicrosoftIdentityWebChallengeUserException)
        {
            if (onMsalUiRequired != null)
            {
                await onMsalUiRequired();
                return string.Empty;
            }
            throw;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}