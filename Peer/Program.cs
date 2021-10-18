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
                    (OpenOptions _) => Task.FromResult("This is where open will happen when it's done"),
                    (ConfigOptions _) => Task.FromResult("This is where config will happen when it's done"),
                    err => Task.FromResult("Saaad"));

            Console.WriteLine(res);
        }

        public async static Task<string> ShowStubAsync(ShowOptions opts)
        {
            var services = new ServiceCollection();

            var configLoader = new ConfigurationService(new List<IRegistrationHandler>
            {
                new GithubWebRegistrationHandler(services)
            });

            Console.WriteLine($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/peer.json");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/peer.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var configResults = configLoader.RegisterProvidersForConfiguration(configuration, services);

            if (configResults.IsError)
            {
                Console.WriteLine("Error in config");
            }

            services.AddSingleton<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton(new ConsoleConfig(inline: true));
            services.AddSingleton<IPullRequestFormatter, CompactFormatter>();
            services.AddSingleton<ISymbolProvider, DefaultEmojiProvider>();
            services.AddSingleton<IPeerApplication, PeerApplication>();
            var p = services.BuildServiceProvider();

            var app = p.GetRequiredService<IPeerApplication>();
            await app.ShowAsync(new Show(), default);

            return string.Empty;
        }
    }
}
