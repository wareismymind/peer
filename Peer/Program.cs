using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Peer.Domain;
using Peer.Domain.Formatters;

namespace Peer
{
    public static class Program
    {
        public static async Task Main(string[] _)
        {
            await Show();
        }

        public static async Task Show()
        {
            var token = Environment.GetEnvironmentVariable("PEER_GH_PAT");
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.Error.WriteLine("PEER_GH_PAT not set");
                Environment.Exit(1);
                return;
            }
            var repoOwners = Environment.GetEnvironmentVariable("PEER_GH_REPO_OWNERS");
            if (string.IsNullOrWhiteSpace(repoOwners))
            {
                Console.Error.WriteLine("PEER_GH_REPO_OWNERS not set");
                Environment.Exit(1);
                return;
            }

            var gqlClient = new GraphQLHttpClient(
                "https://api.github.com/graphql", new NewtonsoftJsonSerializer());
            gqlClient.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token);

            var fetcher = new GitHubRequestFetcher(
                gqlClient, repoOwners.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
            var pullRequests = await fetcher.GetPullRequestsAsync();
            var formatter = new CompactFormatter(new DefaultEmojiProvider());
            var writer = new ConsoleWriter();
            var output = formatter.FormatLines(pullRequests).ToList();
            writer.Display(output, true, default);
        }
    }
}
