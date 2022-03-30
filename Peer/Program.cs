using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Peer.ConfigSections;
using Peer.Domain;
using Peer.Domain.Commands;
using Peer.Domain.Configuration;
using Peer.Domain.Formatters;
using Peer.GitHub;
using Peer.Parsing;
using Peer.Verbs;
using Serilog;
using Serilog.Events;
using wimm.Secundatives;

namespace Peer
{
    public static class Program
    {
        private static readonly string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.json");
        private static readonly string _logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.log");

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
            Log.Logger.Information("Setup complete with success: {Result}", setupResult.IsValue);

            var parser = new Parser(config =>
            {
                config.AutoHelp = true;
                config.AutoVersion = true;
                config.IgnoreUnknownArguments = false;
            });


            var parseResult = parser.ParseArguments<ShowOptions, OpenOptions, ConfigOptions, DetailsOptions>(args);

            if (setupResult.IsError && !parseResult.Is<ConfigOptions>())
            {
                Log.Logger.Error("Setup failed with error: {Error}", setupResult.Error);
                Console.Error.WriteLine(_configErrorMap[setupResult.Error]);
                return;
            }

            if (parseResult.Tag == ParserResultType.Parsed)
            {
                try
                {
                    await parseResult.MapResult(
                        (ShowOptions x) => ShowAsync(x, setupResult.Value, _tcs.Token),
                        (OpenOptions x) => OpenAsync(x, setupResult.Value, _tcs.Token),
                        (ConfigOptions x) => ConfigAsync(x),
                        (DetailsOptions x) => DetailsAsync(x, setupResult.Value, _tcs.Token),
                        err => Task.CompletedTask);
                }
                catch (OperationCanceledException)
                {
                    Log.Logger.Information("Caught operation cancelled exception, exiting");
                }

                return;
            }

            var services = setupResult.Value;
            var text = parseResult switch
            {
                var v when v.Is<ShowOptions>() => GetHelpText<ShowOptions>(parseResult, services.BuildServiceProvider()),
                var v when v.Is<DetailsOptions>() => GetHelpText<DetailsOptions>(parseResult, services.BuildServiceProvider()),
                _ => HelpText.AutoBuild<object>(parseResult)
            };

            Console.Write(text);
        }

        //todo:cn -- maybe pun + indirect here?
        private static HelpText GetHelpText<TOptions>(ParserResult<object> parserResult, IServiceProvider serviceProvider)
        {
            var formatter = serviceProvider.GetRequiredService<IHelpTextFormatter<TOptions>>();
            return formatter.GetHelpText(parserResult);
        }

        public static async Task ShowAsync(ShowOptions opts, IServiceCollection services, CancellationToken token)
        {
            if (opts.Sort != null)
            {
                Log.Information("Sort option exists");
                var sort = SortParser.ParseSortOption(opts.Sort);
                if (sort.IsError)
                {
                    Console.Error.WriteLine($"Failed to parse sort option: {sort.Error}");
                    return;
                }

                services.AddSingleton(sort.Value);
            }

            if (opts.Filter != null)
            {
                Log.Information("filter option exists");
                foreach (var filter in opts.Filter)
                {
                    var parsedFilter = FilterParser.ParseFilterOption(filter);

                    if (parsedFilter.IsError)
                    {
                        Console.Error.WriteLine($"Failed to parse filter option: {parsedFilter.Error}");
                        return;
                    }

                    services.AddSingleton(parsedFilter.Value);
                }
            }

            services.AddSingleton<Show>();
            services.AddSingleton(new ConsoleConfig(inline: !opts.Watch));

            if (opts.Watch)
            {
                services.AddSingleton<WatchShow>();
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<WatchShow>();
                await command.WatchAsync(new ShowArguments(opts.Count), token);
            }
            else
            {
                var provider = services.BuildServiceProvider();
                var command = provider.GetRequiredService<Show>();
                await command.ShowAsync(new ShowArguments(opts.Count), token);
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

            // Parse legacy configuration setting and layer it with default values for newer settings. We'll wait to
            // expose the new settings until we've settled on some configuration patterns.
            // https://github.com/wareismymind/peer/issues/149
            var watchOptions = configuration.GetSection("Peer")
                .Get<WatchOptions>()
                ?? new WatchOptions();

            var showConfig = new ShowConfigSection();
            if (watchOptions.WatchIntervalSeconds != null)
            {
                showConfig.WatchIntervalSeconds = watchOptions.WatchIntervalSeconds;
            }

            services.AddSingleton(showConfig.Into());

            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Information)
                .WriteTo.File(_logFile)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Logger = logger;
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(logger);
            });

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
