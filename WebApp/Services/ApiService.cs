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
            var scopeConfig = _configuration["DownstreamApis:APINoGraph:Scopes"];
            
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(scopeConfig))
            {
                return "Configuration missing for APINoGraph";
            }
            
            var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scopeConfig);
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

    public async Task<string> CallAPIWithGraphAsync()
    {
        try
        {
            var baseUrl = _configuration["APIEndpoints:APIWithGraphBaseUrl"];
            var scopeConfig = _configuration["DownstreamApis:APIWithGraph:Scopes"];
            
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(scopeConfig))
            {
                return "Configuration missing for APIWithGraph";
            }
            
            var scopes = new[] { scopeConfig };
            
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            // Updated endpoint to match ProfileController
            var response = await _httpClient.GetAsync($"{baseUrl}/api/profile/mobile-greeting");
            
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
}