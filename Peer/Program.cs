using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.ConnectorApi;
using Peer.Connectors;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.IO;
using Peer.User;

namespace Peer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var configModel = BuildConfig();

            RegisterServices(services, configModel);
            PeerPullRequestAPI<GithubPullRequestSource> GitpRListApi;
            PeerPullRequestAPI<AzdoPullRequestSource> AzDopRListApi;
            PeerPullRequestAPI<GitlabPullRequestSource> GitlabpRListApi;
            BuildServices(services, out GitpRListApi, out AzDopRListApi, out GitlabpRListApi);

            _ = await GitpRListApi.GetPullRequests();
            _ = await AzDopRListApi.GetPullRequests();
            _ = await GitlabpRListApi.GetPullRequests();

            Console.ReadLine();
        }

        private static AppConfig BuildConfig()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();
            var configModel = config.GetSection("UserConfig").Get<AppConfig>();
            return configModel;
        }

        private static void BuildServices(IServiceCollection services, out PeerPullRequestAPI<GithubPullRequestSource> GitpRListApi, out PeerPullRequestAPI<AzdoPullRequestSource> AzDopRListApi, out PeerPullRequestAPI<GitlabPullRequestSource> GitlabpRListApi)
        {
            GitpRListApi = services
                            .BuildServiceProvider()
                            .GetRequiredService<PeerPullRequestAPI<GithubPullRequestSource>>();
            AzDopRListApi = services
                            .BuildServiceProvider()
                            .GetRequiredService<PeerPullRequestAPI<AzdoPullRequestSource>>();
            GitlabpRListApi = services
                            .BuildServiceProvider()
                            .GetRequiredService<PeerPullRequestAPI<GitlabPullRequestSource>>();
        }

        private static void RegisterServices(IServiceCollection services, AppConfig configModel)
        {
            services.AddTransient<IPullRequestSource<GithubPullRequestSource>, GithubPullRequestSource>();
            services.AddTransient<IPullRequestSource<AzdoPullRequestSource>, AzdoPullRequestSource>();
            services.AddTransient<IPullRequestSource<GitlabPullRequestSource>, GitlabPullRequestSource>();
            services.AddTransient<IClient<UserGithubClient>, UserGithubClient>();
            services.AddTransient<IClient<UserAzdoClient>, UserAzdoClient>();
            services.AddTransient<IClient<UserGitlabClient>, UserGitlabClient>();
            services.AddSingleton(configModel);
            services.AddTransient<PeerPullRequestAPI<GithubPullRequestSource>>();
            services.AddTransient<PeerPullRequestAPI<AzdoPullRequestSource>>();
            services.AddTransient<PeerPullRequestAPI<GitlabPullRequestSource>>();
            services.AddTransient<WritePullRequestTable>();
        }
    }
}
