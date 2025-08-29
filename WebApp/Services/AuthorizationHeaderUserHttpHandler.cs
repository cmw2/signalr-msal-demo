using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace WebApp.Services
{
    public class AuthorizationHeaderUserHttpHandler : DelegatingHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthorizationHeaderUserHttpHandler> _logger;

        public AuthorizationHeaderUserHttpHandler(ITokenAcquisition tokenAcquisition, IConfiguration configuration, ILogger<AuthorizationHeaderUserHttpHandler> logger)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get the scopes from configuration
            var scopes = _configuration.GetSection("DownstreamApis:APIWithGraph:Scopes").Get<string[]>();
            if (scopes == null || scopes.Length == 0)
            {
                throw new InvalidOperationException("DownstreamApis:APIWithGraph:Scopes configuration is missing.");
            }


            string accessToken;

            try
            {
                // Use ITokenAcquisition to get an access token for the logged-in user
                accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error acquiring access token for app.");

                // Handle exceptions related to token acquisition
                throw new InvalidOperationException("Failed to acquire access token.", ex);
            }


            // Attach the token to the Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Continue the request pipeline
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
