using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;

namespace AzureDevOpsTokenGenerator
{
    public sealed class AzureDevOpsClient : IDisposable
    {
        public string OrganizationName { get; private set; }
        public VssConnection VssConnection { get; private set; }
        public AuthenticationResult AuthenticationResult { get; private set; }

        public AzureDevOpsClient(string organization)
        {
            this.OrganizationName = organization;
        }
        
        // Default to AAD (developer workstation scenario)
        public async Task ConnectAsync()
        {
            string tenant = "microsoft.com";

            string aadAuthority = $"https://login.windows.net/{tenant}";

            // Fixed static resource Guid for Azure Devops
            string aadResource = "499b84ac-1321-427f-aa17-267ca6975798";

            // MSA client ID if you don't have an application ID regsitered with Azure
            string aadClientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";

            // Login now
            AuthenticationContext authCtx = new AuthenticationContext(aadAuthority);
            string aadUser = $"{Environment.UserName}@{tenant}";
            UserCredential userCredential = new UserCredential(aadUser);
            Console.WriteLine($"Logging in as {userCredential.UserName}");
            this.AuthenticationResult = await authCtx.AcquireTokenAsync(aadResource, aadClientId, userCredential);

            // Connect to Azure DevOps
            Uri collectionUri = new Uri($"https://dev.azure.com/{this.OrganizationName}");
            VssOAuthAccessTokenCredential oAuthCredentials = new VssOAuthAccessTokenCredential(this.AuthenticationResult.AccessToken);
            VssCredentials vssCredentials = oAuthCredentials;
            this.VssConnection = new VssConnection(collectionUri, vssCredentials);
        }

        public void Dispose()
        {
            this.VssConnection?.Dispose();
        }
    }
}
