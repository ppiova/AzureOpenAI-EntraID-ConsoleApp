using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace AzureAOAI_EntraID_ConsoleApp
{
    // Class for Azure authentication
    public class AzureAuthenticationService
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _resource;

        public AzureAuthenticationService(string tenantId, string clientId, string clientSecret, string resource)
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _resource = resource;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Create a confidential client application with the provided credentials
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_clientId)
                .WithClientSecret(_clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{_tenantId}"))
                .Build();

            // Define the scopes for the token request
            string[] scopes = new string[] { _resource };

            // Acquire the token for the client
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
    }
}
