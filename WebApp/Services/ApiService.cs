namespace WebApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> CallAPINoGraphAsync()
    {
        try
        {
            var baseUrl = _configuration["APIEndpoints:APINoGraphBaseUrl"];
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

    public Task<string> CallAPIWithGraphAsync()
    {
        try
        {
            // TODO: Implement token acquisition and protected API call
            // This would require proper authentication flow to be set up
            var baseUrl = _configuration["APIEndpoints:APIWithGraphBaseUrl"];
            
            // Placeholder for now - in real implementation, you would:
            // 1. Acquire access token for the API
            // 2. Add Authorization header
            // 3. Make the API call
            
            return Task.FromResult("Placeholder for APIWithGraph call - requires proper authentication setup");
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Exception: {ex.Message}");
        }
    }
}