using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain;
using Peer.Domain.Commands;
using Peer.Domain.Configuration;
using Peer.Domain.Formatters;
using Peer.GitHub;
using Peer.Parsing;
using Peer.Verbs;
using wimm.Secundatives;

namespace Peer
{
    public static class Program
    {
        private static readonly string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.json");

        private static readonly Dictionary<ConfigError, string> _configErrorMap = new()
        {
            [ConfigError.InvalidProviderValues] = "One or more providers have invalid configuration",
            [ConfigError.NoProvidersConfigured] = "No providers are configured! Run 'peer config' to get started",
            [ConfigError.ProviderNotMatched] = "Provider was not recognized, make sure you're using one of supported providers!"
        };

        private const string _configHelp = @"
{
  ""Providers"": {
    //The type of the provider you're configuring (currently there's only github!)
    ""github"": [{
        ""Name"": ""required: a friendly name for this provider"",
        ""Configuration"": {
          ""AccessToken"": ""required: your API token"",
          ""Username"": ""optional: the github username you're interested in investigating, alternatively we'll fetch yours from the api"",
          //optional: Orgs can be either be traditional (github, wareismymind) or a username for user's repos 
          // if left empty we'll look at all orgs available to your user
          ""Orgs"": [""myorg"", ""wareismymind"", ""someuser""],
          //optional: Orgs that you'd like to exclude from the output, only really makes sense if no orgs are set
          ""ExcludedOrgs"": []
        }
    }]
  }
}
";

        private static readonly CancellationTokenSource _tcs = new();

        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (evt, eventArgs) =>
            {
                Console.WriteLine("Stopping...");
                _tcs.Cancel();
            };

            Console.OutputEncoding = Encoding.UTF8;

            await Parser.Default.ParseArguments<ShowOptions, OpenOptions, ConfigOptions>(args)
                .MapResult(
                    (ShowOptions x) => ShowAsync(x, _tcs.Token),
                    (OpenOptions x) => OpenAsync(x, _tcs.Token),
                    (ConfigOptions x) => ConfigAsync(x),
                    err => Task.CompletedTask);
        }

        public static async Task ShowAsync(ShowOptions opts, CancellationToken token)
        {
            var setupResult = SetupServices();

            if (setupResult.IsError)
            {
                Console.Error.WriteLine(_configErrorMap[setupResult.Error]);
                return;
            }

            if (opts.Sort != null)
            {
                var sort = SortParser.ParseSortOption(opts.Sort);
                if (sort.IsError)
                {
                    Console.Error.WriteLine($"Failed to parse sort option: {sort.Error}");
                }

                setupResult.Value.AddSingleton<ISorter<PullRequest>>(sort.Value);
            }

            var p = setupResult.Value.BuildServiceProvider();
            var app = p.GetRequiredService<IPeerApplication>();
            await app.ShowAsync(new Show(), token);
        }

        public static async Task OpenAsync(OpenOptions opts, CancellationToken token)
        {
            var setupResult = SetupServices();

            if (setupResult.IsError)
            {
                Console.Error.WriteLine(_configErrorMap[setupResult.Error]);
                return;
            }

            var provider = setupResult.Value.BuildServiceProvider();
            var app = provider.GetRequiredService<IPeerApplication>();
            await app.OpenAsync(new Open(opts.Partial ?? ""), token);

        }

        public static Task ConfigAsync(ConfigOptions _)
        {
            Console.Error.WriteLine("Hey lets get you set up and working with Peer!");
            Console.Error.WriteLine($"Toss the following into this location: {_configFile} and fill in values for your github account");
            Console.WriteLine(_configHelp);
            return Task.CompletedTask;
        }

        private static Result<IServiceCollection, ConfigError> SetupServices()
        {
            var services = new ServiceCollection();

            var configLoader = new ConfigurationService(new List<IRegistrationHandler>
            {
                new GitHubWebRegistrationHandler(services)
            });

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(_configFile, optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configResults = configLoader.RegisterProvidersForConfiguration(configuration, services);

            if (configResults.IsError)
            {
                return configResults.Error;
            }

            services.AddSingleton<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton<IPullRequestFormatter, CompactFormatter>();
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IPeerApplication, PeerApplication>();
            services.AddSingleton<IOSInfoProvider, OSInfoProvider>();
            services.AddSingleton(new ConsoleConfig(inline: true));
            return services;
        }
    }
}
