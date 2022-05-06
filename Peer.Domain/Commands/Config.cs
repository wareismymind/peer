using System;
using System.IO;
using System.Threading.Tasks;

namespace Peer.Domain.Commands
{
    public class Config
    {
        private static readonly string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.json");

        private const string _configHelp = @"
{
    ""Peer"": {
        // The amount of time to wait for the show command to fetch pull request info (default: 10)
        ""ShowTimeoutSeconds"": 30
        // The amount of time between calls to providers when using the --watch flag (default: 30)
        ""WatchIntervalSeconds"": 15
    },
    // Pull request provider configurations organized by type (currently there's only github!)
    ""Providers"": {
        // A list of GitHub pull request provider configurations
        ""github"": [{
            // A friendly name for this provider (required)
            ""Name"": ""GitHub-Work"",
            ""Configuration"": {
                // An API token with permission to read issues (required)
                // You will need to configure SSO on the PAT to see pull requests from organizations that require it
                ""AccessToken"": """",
                // The GitHub username you're interested in investigating (optional)
                // If not provided we'll fetch the username associated with the AccessToken from the API
                ""Username"": """",
                // A list of organizations or other GitHub users whose repos to include pull requests from (default: [])
                // If the list is empty then we'll include all visible requests regardless of the repo owner
                ""Orgs"": [""myorg"", ""wareismymind"", ""someuser""],
                // A list of organizations or other GitHub users whose repos will be excluded when searching for pull requests (default: [])
                // Use this option as an alternative to `Orgs`
                ""ExcludedOrgs"": [],
                // The number of pull requests to include in your search results (min: 1, max: 100, default: 20)
                ""Count"": 7
            }
        }]
    }
}
";
        public static Task ConfigAsync()
        {
            Console.Error.WriteLine("Hey lets get you set up and working with Peer!");
            Console.Error.WriteLine($"Toss the following into this location: {_configFile} and fill in values for your github account");
            Console.WriteLine(_configHelp);
            return Task.CompletedTask;
        }
    }
}
