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
        public string ProviderKey => ProviderConstants.Github;

        public Result<None, RegistrationError> Register(IConfigurationSection config, IServiceCollection services)
        {
            var childConfigs = config.GetChildren().Select(x => x.Get<GitHubHandlerConfig>())
                            .Where(x => x != null)
                            .Select(x => x!.Into())
                            .Collect();

            if (childConfigs.IsError)
            {
                //TODO:CN -- log here;
                return RegistrationError.BadConfig;
            }

            foreach (var gitHubConfig in childConfigs.Value)
            {
                services.AddSingleton(sp =>
                {
                    var client = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
                    client.HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", gitHubConfig.AccessToken);
                    return new NamedGraphQLBinding(gitHubConfig.Name, client);
                });

                services.AddSingleton<IPullRequestFetcher, GitHubRequestFetcher>(sp =>
                {
                    var client = sp.GetRequiredService<IGraphQLClientFactory>().GetClient(gitHubConfig.Name);
                    return new GitHubRequestFetcher(client, gitHubConfig);
                });
            }

            services.TryAddSingleton<IGraphQLClientFactory, GraphQLClientFactory>();

            return Maybe.None;
        }
    }
}
