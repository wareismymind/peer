using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps.AppBuilder;
using Peer.ConfigSections;
using Peer.Domain;
using Peer.Domain.Commands;
using Peer.Domain.Configuration;
using Peer.Domain.Formatters;
using Peer.Domain.Util;
using Peer.GitHub;
using Peer.Handlers;
using Peer.Parsing;
using Peer.Verbs;
using wimm.Secundatives;

namespace Peer
{
    public static class Program
    {
        private static readonly CancellationTokenSource _tcs = new();

        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public static async Task<int> Main(string[] args)
        {
            Console.CancelKeyPress += (_, _) =>
            {
                Console.WriteLine("Stopping...");
                _tcs.Cancel();
            };

            Console.OutputEncoding = Encoding.UTF8;

            var builder = new AppBuilder(new ServiceCollection())
                .WithParseTimeServiceConfig(SetupParseTimeServices)
                .WithSharedRuntimeConfig(SetupServices)
                .WithConfiguration(configBuilder =>
                {
                    var configPath = Environment.GetEnvironmentVariable("PEER_CONFIGPATH")
                                     ?? Constants.DefaultConfigPath;

                    if (configPath.StartsWith("~/"))
                    {

                        configPath = Path.Join(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            configPath[2..]);
                    }

                    var editor = Environment.GetEnvironmentVariable("EDITOR");
                    var inMemory = new Dictionary<string, string?>
                    {
                        ["Peer:Environment:ConfigPath"] = configPath,
                        ["Peer:Environment:Editor"] = editor
                    };

                    configBuilder
                        .AddInMemoryCollection(inMemory)
                        .AddJsonFile(configPath, optional: true)
                        .SetFileLoadExceptionHandler(x => x.Ignore = true)
                        .AddEnvironmentVariables();
                });

            builder.WithVerb<ConfigOptions>(
                    conf =>
                    {
                        conf.WithSubVerb<ConfigInitOptions>(x => x.WithHandler<ConfigInitHandler>())
                            .WithSubVerb<ConfigShowOptions>(x => x.WithHandler<ConfigShowHandler>())
                            .WithSubVerb<ConfigEditOptions>(x => x.WithHandler<ConfigEditHandler>());
                    })
                .WithVerb<ShowOptions>(
                    conf =>
                    {
                        conf.WithCustomHelp<ShowHelpTextFormatter>()
                            .WithActionHandler(ShowAsync)
                            .WithRunTimeConfig<ProviderLoader>();

                    })
                .WithVerb<OpenOptions>(
                    conf =>
                    {
                        conf.WithActionHandler(OpenAsync)
                            .WithRunTimeConfig<ProviderLoader>();
                    })
                .WithVerb<DetailsOptions>(
                    conf =>
                    {
                        conf.WithCustomHelp<DetailsHelpTextFormatter>()
                            .WithActionHandler(DetailsAsync)
                            .WithRunTimeConfig<ProviderLoader>();
                    });


            var p = builder.Build();

            return await p.RunAsync(args);
        }

        private static async Task<int> ShowAsync(ShowOptions opts, IServiceCollection services, CancellationToken token)
        {
            services.AddSingleton(
                sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    // Parse legacy configuration setting and layer it with default values for newer settings. We'll wait to
                    // expose the new settings until we've settled on some configuration patterns.
                    // https://github.com/wareismymind/peer/issues/149
                    var watchOptions = config.GetSection("Peer")
                                            ?.Get<PeerOptions>()
                                            ?? new PeerOptions();

                    var showConfig = new ShowConfigSection();
                    if (watchOptions.WatchIntervalSeconds != null)
                    {
                        showConfig.WatchIntervalSeconds = watchOptions.WatchIntervalSeconds;
                        showConfig.TimeoutSeconds = watchOptions.ShowTimeoutSeconds;
                    }

                    return showConfig.Into();
                });

            if (opts.Sort != null)
            {
                var sort = SortParser.ParseSortOption(opts.Sort);
                if (sort.IsError)
                {
                    await Console.Error.WriteLineAsync($"Failed to parse sort option: {sort.Error}");
                    return 1;
                }

                services.AddSingleton(sort.Value);
            }

            if (opts.Filter != null)
            {
                foreach (var filter in opts.Filter)
                {
                    var parsedFilter = FilterParser.ParseFilterOption(filter);

                    if (parsedFilter.IsError)
                    {
                        await Console.Error.WriteLineAsync($"Failed to parse filter option: {parsedFilter.Error}");
                        return 1;
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
                var res = await command.ShowAsync(new ShowArguments(opts.Count), token);
                if (res.IsError)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static async Task<int> OpenAsync(OpenOptions opts, IServiceCollection services, CancellationToken token)
        {
            var parseResult = PartialIdentifier.Parse(opts.Partial!);

            if (parseResult.IsError)
            {
                await Console.Error.WriteLineAsync(
                    $"Failed to parse partial identifier '{opts.Partial}' with error: {parseResult.Error}");
                return 1;
            }

            services.AddSingleton<Open>();
            var provider = services.BuildServiceProvider();
            var command = provider.GetRequiredService<Open>();

            var result = await command.OpenAsync(new OpenArguments(parseResult.Value), token);

            if (result.IsError)
            {
                await Console.Error.WriteLineAsync(
                    $"Partial identifier '{opts.Partial}' failed with error: {result.Error}");
                return 1;
            }

            return 0;
        }

        private static async Task<int> DetailsAsync(
            DetailsOptions opts,
            IServiceCollection services,
            CancellationToken token)
        {
            var parseResult = PartialIdentifier.Parse(opts.Partial!);

            if (parseResult.IsError)
            {
                await Console.Error.WriteLineAsync(
                    $"Failed to parse partial identifier '{opts.Partial}' with error: {parseResult.Error}");
                return 1;
            }

            services.AddSingleton<Details>();
            services.AddSingleton(new ConsoleConfig(inline: true));

            var provider = services.BuildServiceProvider();
            var command = provider.GetRequiredService<Details>();

            var res = await command.DetailsAsync(new DetailsArguments(parseResult.Value), token);

            if (!res.IsError)
            {
                return 0;
            }

            await Console.Error.WriteLineAsync($"Partial identifier '{opts.Partial}' failed with error: {res.Error}");
            return 1;
        }

        private static Result<IServiceCollection, ConfigError> SetupParseTimeServices(IServiceCollection services)
        {
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<ICheckSymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IRegistrationHandler, GitHubWebRegistrationHandler>();
            services.AddSingleton(
                sp => sp.GetRequiredService<IConfiguration>()
                    .GetSection("Peer:Environment")
                    .Get<PeerEnvironmentOptions>()!);
            return new Result<IServiceCollection, ConfigError>(services);
        }

        [RequiresUnreferencedCode("Calls Microsoft.Extensions.Configuration.IConfiguration.Get<Peer.Parsing.PeerOptions>()")]
        private static Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services)
        {
            services.AddSingleton<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton<IListFormatter, CompactFormatter>();
            services.AddSingleton<IDetailsFormatter, DetailsFormatter>();
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<ICheckSymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IOSInfoProvider, OSInfoProvider>();
            services.AddSingleton<IPullRequestService, PullRequestService>();
            services.AddSingleton<IFileOperations, FileOperations>();

            return new Result<IServiceCollection, ConfigError>(services);
        }
    }
}
