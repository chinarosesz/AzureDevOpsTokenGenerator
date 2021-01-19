using AzureDevOpsTokenGenerator;
using CommandLine;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;

namespace AzureDevOps.TokenGenerator.Net
{
    /// <summary>
    /// Run the exe to get help options. 
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Parse command line
            CommandOptions parsedOptions = Program.ParseArguments(args);
            if (parsedOptions == null) { return; }

            // Connect to Azure DevOps client
            AzureDevOpsClient client = new AzureDevOpsClient(parsedOptions.Organization);
            await client.ConnectAsync();

            // Display token information
            Console.WriteLine($"Organization: {client.VssConnection.Uri}");
            Console.WriteLine($"AccessToken: {client.AuthenticationResult.AccessToken}");
            Console.WriteLine($"AccessTokenType: {client.AuthenticationResult.AccessTokenType}");
            Console.WriteLine($"ExpiresOn: {client.AuthenticationResult.ExpiresOn}");
            Console.WriteLine($"TenantId: {client.AuthenticationResult.TenantId}");
            Console.WriteLine($"GivenName: {client.AuthenticationResult.UserInfo?.GivenName}");
            Console.WriteLine($"FamilyName: {client.AuthenticationResult.UserInfo?.FamilyName}");
            Console.WriteLine($"Authority: {client.AuthenticationResult.Authority}");

            // Get list of projects and display
            ProjectHttpClient projectHttpClient = await client.VssConnection.GetClientAsync<ProjectHttpClient>();
            IPagedList<TeamProjectReference> projects = await projectHttpClient.GetProjects(top: 500);
            Console.Write("Projects: ");
            foreach (TeamProjectReference project in projects)
            {
                Console.Write($"{project.Name} ");
            }
        }

        private static CommandOptions ParseArguments(string[] args)
        {
            // Parse command line options
            CommandOptions commandOptions = new CommandOptions();
            ParserResult<CommandOptions> results = Parser.Default.ParseArguments<CommandOptions>(args);

            // Map results after parsing
            results.MapResult<CommandOptions, object>((CommandOptions opts) => commandOptions = opts, (errs) => 1);

            // Return null if not able to parse
            if (results.Tag == ParserResultType.NotParsed)
            {
                return null;
            }

            return commandOptions;
        }
    }
}
