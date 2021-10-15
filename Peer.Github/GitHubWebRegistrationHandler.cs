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
    public class GithubWebRegistrationHandler : IRegistrationHandler
    {
        private readonly IServiceCollection _services;
        public string ProviderKey => "github";

        public GithubWebRegistrationHandler(IServiceCollection services)
        {
            _services = services;
        }

        public Result<None, RegistrationError> Register(IConfigurationSection config)
        {
            var childConfigs = config.GetChildren().Select(x => x.Get<GithubHandlerConfig>())
                            .Select(x => x.Into())
                            .Collect();

            if (childConfigs.IsError)
            {
                //TODO:CN -- log here;
                return RegistrationError.BadConfig;
            }

            foreach (var githubConfig in childConfigs.Value)
            {
                _services.AddSingleton(sp =>
                {
                    var client = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
                    client.HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", githubConfig.AccessToken);
                    return new NamedGraphQLBinding(githubConfig.Name, client);
                });

                _services.AddSingleton<IPullRequestFetcher, GitHubRequestFetcher>(sp =>
                {
                    var client = sp.GetRequiredService<IGraphQLClientFactory>().GetClient(githubConfig.Name);
                    return new GitHubRequestFetcher(client, githubConfig);
                });
            }

            _services.TryAddSingleton<IGraphQLClientFactory, GraphQLClientFactory>();

            return Maybe.None;
        }
    }
}
