using Microsoft.Identity.Web;

namespace WebApp.Services;

public class ApiServiceWithHandler  
{
    private readonly HttpClient _appHttpClient;
    private readonly HttpClient _userHttpClient;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ApiServiceWithHandler(IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _appHttpClient = httpClientFactory.CreateClient("APINoGraphHandler");
        _userHttpClient = httpClientFactory.CreateClient("APIWithGraphHandler");
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<string> CallAPINoGraphAsync()
    {
        try
        {
            var response = await _appHttpClient.GetAsync("api/hello");
            
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
            var response = await _userHttpClient.GetAsync("api/profile/mobile-greeting");
            
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