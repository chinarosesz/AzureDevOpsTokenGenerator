using CommandLine;

namespace AzureDevOpsTokenGenerator
{
    public class CommandOptions
    {
        [Option("organization", Required = true, HelpText = "The name of Azure DevOps organization. For example: https://dev.azure.com/{organization}, pass in only the name and not the URL")]
        public string Organization { get; set; }
    }
}