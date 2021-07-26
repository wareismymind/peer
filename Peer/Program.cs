using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.ConnectorApi;
using Peer.Connectors;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var configModel = config.GetSection("UserConfig").Get<ConfigModel>();

            // TODO: Create generic interface for IPullRequestSource
            services.AddTransient<IPullRequestSource, GithubPullRequestSource>();
            services.AddTransient<PRListApi>();
            services.AddTransient<IClient<GithubClient>, GithubClient>();
            services.AddTransient<IClient<AzdoClient>, AzdoClient>();
            services.AddSingleton(configModel);

            var pRListApi = services
                .BuildServiceProvider()
                .GetRequiredService<PRListApi>();

            _ = await pRListApi.GetPullRequests();

            Console.WriteLine(configModel.Github.ProductHeaderValue);
        }
    }
}
