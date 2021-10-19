using System.Linq;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peer.Domain;
using Peer.Domain.Configuration;
using Peer.Domain.GraphQL;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.GitHub
{
    public class GitHubWebRegistrationHandler : IRegistrationHandler
    {
        private readonly IServiceCollection _services;
        public string ProviderKey => ProviderConstants.Github;

        public GitHubWebRegistrationHandler(IServiceCollection services)
        {
            _services = services;
        }

        public Result<None, RegistrationError> Register(IConfigurationSection config)
        {
            var childConfigs = config.GetChildren().Select(x => x.Get<GitHubHandlerConfig>())
                            .Select(x => x.Into())
                            .Collect();

            if (childConfigs.IsError)
            {
                //TODO:CN -- log here;
                return RegistrationError.BadConfig;
            }

            foreach (var gitHubConfig in childConfigs.Value)
            {
                _services.AddSingleton(sp =>
                {
                    var client = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
                    client.HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", gitHubConfig.AccessToken);
                    return new NamedGraphQLBinding(gitHubConfig.Name, client);
                });

                _services.AddSingleton<IPullRequestFetcher, GitHubRequestFetcher>(sp =>
                {
                    var client = sp.GetRequiredService<IGraphQLClientFactory>().GetClient(gitHubConfig.Name);
                    return new GitHubRequestFetcher(client, gitHubConfig);
                });
            }

            _services.TryAddSingleton<IGraphQLClientFactory, GraphQLClientFactory>();

            return Maybe.None;
        }
    }
}
