using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain;
using Peer.Domain.Configuration;
using Peer.GitHub;
using Peer.Verbs;

namespace Peer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var res = await Parser.Default.ParseArguments<ShowOptions, OpenOptions>(args)
                .MapResult(
                    (ShowOptions x) => ShowStubAsync(x),
                    (OpenOptions _) => Task.FromResult("This is where open will happen when it's done"),
                    (ConfigOptions _) => Task.FromResult("This is where config will happen when it's done"),
                    err => Task.FromResult("Saaad"));

            Console.WriteLine(res);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public static Task<string> ShowStubAsync(ShowOptions opts)
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
            _ = p.GetRequiredService<IPullRequestFetcher>();

            return Task.FromResult("ok");
        }
    }
}
