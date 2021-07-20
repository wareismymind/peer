using System;
using Microsoft.Extensions.DependencyInjection;
using Peer.ConnectorApi;
using Peer.Connectors;
using Peer.Domain;

namespace Peer
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddTransient<IPullRequestSource, GithubPullRequestSource>();

            var sp = services.BuildServiceProvider();

            var prListApi = sp.GetRequiredService<PRListApi>();

            Console.WriteLine(prListApi);
        }
    }
}
