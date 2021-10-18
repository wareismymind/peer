using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain;
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

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("Test.json", optional: false)
                .Build();
            _ = configLoader.RegisterProvidersForConfiguration(configuration, services);

            var p = services.BuildServiceProvider();
            var fetcher = p.GetRequiredService<IPullRequestFetcher>();

            var pullRequests = await fetcher.GetPullRequestsAsync();
            var formatter = new CompactFormatter(new DefaultEmojiProvider());
            var writer = new ConsoleWriter();
            var output = formatter.FormatLines(pullRequests).ToList();
            writer.Display(output, true, default);

            return string.Empty;
        }
    }
}
