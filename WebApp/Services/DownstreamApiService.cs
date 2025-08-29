using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace WebApp.Services;

public class DownstreamApiService
{
    private readonly IDownstreamApi _downstreamApi;
    private readonly string _scope;
    public DownstreamApiService(IDownstreamApi downstreamApi, IConfiguration config)
    {
        _downstreamApi = downstreamApi;
        _scope = config["DownstreamApis:APINoGraph:Scopes"] ?? string.Empty;
    }

    public async Task<string> CallAPINoGraphAsync()
    {
        try
        {
            var result = await _downstreamApi.CallApiForAppAsync("APINoGraph", options =>
            {
                options.RelativePath = "api/hello";
            });
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return $"Error: {result.StatusCode} - {result.ReasonPhrase}";
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
            var result = await _downstreamApi.CallApiForUserAsync("APIWithGraph", options =>
            {
                options.RelativePath = "api/profile/mobile-greeting";
            });
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return $"Error: {result.StatusCode} - {result.ReasonPhrase}";
        }
        catch (Microsoft.Identity.Web.MicrosoftIdentityWebChallengeUserException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
