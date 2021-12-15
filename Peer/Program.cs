using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
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

            var setupResult = SetupServices();

            if (setupResult.IsError)
            {
                Console.Error.WriteLine(_configErrorMap[setupResult.Error]);
                return;
            }
            var services = setupResult.Value;
            var parser = new Parser(config =>
            {
                config.AutoHelp = true;
                config.AutoVersion = true;
                config.IgnoreUnknownArguments = false;
            });


            var pr = parser.ParseArguments<ShowOptions, OpenOptions, ConfigOptions, DetailsOptions>(args);

            if (pr.Tag == ParserResultType.Parsed)
            {
                await pr.MapResult(
                    (ShowOptions x) => ShowAsync(x, services, _tcs.Token),
                    (OpenOptions x) => OpenAsync(x, services, _tcs.Token),
                    (ConfigOptions x) => ConfigAsync(x),
                    (DetailsOptions x) => DetailsAsync(x, services, _tcs.Token),
                    err => { err.Output(); Console.WriteLine("Doot"); return Task.CompletedTask; });
            }


            var text = pr switch
            {
                var v when v.Is<ShowOptions>() => GetHelpText<ShowOptions>(pr, services.BuildServiceProvider()),
                var v when v.Is<DetailsOptions>() => GetHelpText<DetailsOptions>(pr, services.BuildServiceProvider()),
                _ => HelpText.AutoBuild<object>(pr)
            };

            Console.Write(text);
        }

        //todo:cn -- maybe pun + indirect here?
        private static HelpText GetHelpText<TOptions>(ParserResult<object> parserResult, IServiceProvider serviceProvider)
        {
            var formatter = serviceProvider.GetRequiredService<IHelpTextFormatter<TOptions>>();
            return formatter.GetDetailsHelpText(parserResult);
        }

        public static async Task ShowAsync(ShowOptions opts, IServiceCollection services, CancellationToken token)
        {
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
            services.AddSingleton(new ConsoleConfig(inline: !opts.Watch));

            if (opts.Watch)
            {
                services.AddSingleton<WatchShow>();
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<WatchShow>();
                var watchOpts = provider.GetRequiredService<WatchOptions>();
                var args = watchOpts.Into();
                await command.WatchAsync(args, new ShowArguments(), token);
            }
            else
            {
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<Show>();
                await command.ShowAsync(new ShowArguments(), token);
            }
        }

        public static async Task OpenAsync(OpenOptions opts, IServiceCollection services, CancellationToken token)
        {
            var parseResult = PartialIdentifier.Parse(opts.Partial!);

            if (parseResult.IsError)
            {
                Console.Error.WriteLine(
                    $"Failed to parse partial identifier '{opts.Partial}' with error: {parseResult.Error}");
                return;
            }

            services.AddSingleton<Open>();
            var provider = services.BuildServiceProvider();
            var command = provider.GetRequiredService<Open>();

            var result = await command.OpenAsync(new OpenArguments(parseResult.Value), token);

            if (result.IsError)
            {
                Console.Error.WriteLine($"Partial identifier '{opts.Partial}' failed with error: {result.Error}");
            }
        }

        public static async Task DetailsAsync(DetailsOptions opts, IServiceCollection services, CancellationToken token)
        {
            var parseResult = PartialIdentifier.Parse(opts.Partial!);

            if (parseResult.IsError)
            {
                Console.Error.WriteLine(
                    $"Failed to parse partial identifier '{opts.Partial}' with error: {parseResult.Error}");
                return;
            }

            services.AddSingleton<Details>();
            services.AddSingleton(new ConsoleConfig(inline: true));

            var provider = services.BuildServiceProvider();
            var command = provider.GetRequiredService<Details>();

            var res = await command.DetailsAsync(new DetailsArguments(parseResult.Value), token);

            if (res.IsError)
            {
                Console.Error.WriteLine($"Partial identifier '{opts.Partial}' failed with error: {res.Error}");
            }
        }

        public static async Task ConfigAsync(ConfigOptions _)
        {
            await Config.ConfigAsync();
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

            var watchOptions = configuration.GetSection("Peer")
                .Get<WatchOptions>()
                ?? new WatchOptions();

            if (watchOptions != null)
            {
                services.AddSingleton(watchOptions);
            }

            services.AddSingleton<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton<IListFormatter, CompactFormatter>();
            services.AddSingleton<IDetailsFormatter, DetailsFormatter>();
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<ICheckSymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IOSInfoProvider, OSInfoProvider>();
            services.AddSingleton<IPullRequestService, PullRequestService>();

            services.AddSingleton<IHelpTextFormatter<ShowOptions>, ShowHelpTextFormatter>();
            services.AddSingleton<IHelpTextFormatter<DetailsOptions>, DetailsHelpTextFormatter>();
            return services;
        }
    }
}
