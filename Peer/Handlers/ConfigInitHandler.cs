using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps;
using Peer.ConfigSections;
using Peer.Parsing;
using Peer.Verbs;

namespace Peer.Handlers;

public class ConfigInitHandler : IHandler<ConfigInitOptions>
{
    private readonly PeerEnvironmentOptions _opts;

    private const string _defaultConfig = @"
{
    ""Peer"": {
        // The amount of time to wait for the show command to fetch pull request info (default: 10)
        ""ShowTimeoutSeconds"": 30,
        // The amount of time between calls to providers when using the --watch flag (default: 30)
        ""WatchIntervalSeconds"": 15
    },
    // Pull request provider configurations organized by type (currently there's only github!)
    ""Providers"": {
        // A list of GitHub pull request provider configurations
        ""github"": [{
            // A friendly name for this provider (required)
            ""Name"": ""Github"",
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
                ""Count"": 30
            }
        }]
    }
}
";

    public ConfigInitHandler(PeerEnvironmentOptions opts)
    {
        _opts = opts;
    }

    public async Task<int> HandleAsync(ConfigInitOptions opts, IServiceCollection services, CancellationToken token = default)
    {
        if (File.Exists(_opts.ConfigPath) && !opts.Force)
        {
            var forceName = opts.GetOptionLongName(nameof(opts.Force));
            Console.WriteLine($"You already have a config file! If you want it overwritten use the --{forceName} option");
            return 1;
        }

        await using var file = File.Create(_opts.ConfigPath);
        await using var writer = new StreamWriter(file);
        await writer.WriteAsync(_defaultConfig);
        return 0;
    }
}
