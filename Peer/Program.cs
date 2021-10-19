using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain;
using Peer.Domain.Commands;
using Peer.Domain.Configuration;
using Peer.Domain.Formatters;
using Peer.GitHub;
using Peer.Verbs;

namespace Peer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var res = await Parser.Default.ParseArguments<ShowOptions, OpenOptions>(args)
                .MapResult(
                    (ShowOptions x) => ShowStubAsync(x),
                    (OpenOptions x) => OpenAsync(x),
                    (ConfigOptions _) => Task.FromResult("This is where config will happen when it's done"),
                    err => Task.FromResult("Saaad"));

            Console.WriteLine(res);
        }

        public static async Task<string> ShowStubAsync(ShowOptions opts)
        {

            var services = SetupServices();
            services.AddSingleton(new ConsoleConfig(inline: true));
            var p = services.BuildServiceProvider();
            var app = p.GetRequiredService<IPeerApplication>();
            await app.ShowAsync(new Show(), default);

            return string.Empty;
        }

        public static async Task<string> OpenAsync(OpenOptions opts)
        {
            var services = SetupServices();
            services.AddSingleton(new ConsoleConfig(inline: true));
            var p = services.BuildServiceProvider();
            var app = p.GetRequiredService<IPeerApplication>();

            await app.OpenAsync(new Open(opts.Partial), default);
            return "ooook";
        }

        private static IServiceCollection SetupServices()
        {
            var services = new ServiceCollection();

            var configLoader = new ConfigurationService(new List<IRegistrationHandler>
            {
                new GitHubWebRegistrationHandler(services)
            });

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/peer.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configResults = configLoader.RegisterProvidersForConfiguration(configuration, services);

            if (configResults.IsError)
            {
                Console.Error.WriteLine($"Error in config: {configResults.Error}");
            }

            services.AddSingleton<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton<IPullRequestFormatter, CompactFormatter>();
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IPeerApplication, PeerApplication>();
            services.AddSingleton<IOSInfoProvider, OSInfoProvider>();

            return services;
        }
    }
}
