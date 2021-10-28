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

            var services = setupResult.Value;

            if (opts.Sort != null)
            {
                var sort = SortParser.ParseSortOption(opts.Sort);
                if (sort.IsError)
                {
                    Console.Error.WriteLine($"Failed to parse sort option: {sort.Error}");
                    return;
                }

                services.AddSingleton(sort.Value);
            }

            services.AddSingleton<Show>();
            services.AddSingleton(new ConsoleConfig(inline: opts.Watch == null));

            if (opts.Watch != null)
            {
                services.AddSingleton<WatchShow>();
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<WatchShow>();
                var ts = TimeSpan.FromSeconds(opts.Watch.Value);
                await command.WatchAsync(new WatchArguments(ts), new ShowArguments(), token);
            }
            else
            {
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<Show>();
                await command.ShowAsync(new ShowArguments(), token);
            }
        }

        public static async Task OpenAsync(OpenOptions opts, CancellationToken token)
        {
            var setupResult = SetupServices();

            if (setupResult.IsError)
            {
                Console.Error.WriteLine(_configErrorMap[setupResult.Error]);
                return;
            }
            var services = setupResult.Value;
            services.AddSingleton<Open>();
            var provider = services.BuildServiceProvider();
            var command = provider.GetRequiredService<Open>();
            var result = await command.OpenAsync(new OpenArguments(opts.Partial ?? ""), token);

            if (result.IsError)
            {
                Console.Error.WriteLine($"Partial identifier '{opts.Partial}' failed with error: {result.Error}");
            }
        }

        public static async Task ConfigAsync(ConfigOptions _)
        {
            var config = new Config();
            await config.ConfigAsync();
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
            services.AddSingleton<IOSInfoProvider, OSInfoProvider>();
            return services;
        }
    }
}
