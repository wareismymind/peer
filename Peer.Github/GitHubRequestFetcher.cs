using GraphQL.Client.Http;
using Peer.Domain;

namespace Peer.GitHub
{
    public class GitHubRequestFetcher : IPullRequestFetcher
    {
        private readonly GraphQLHttpClient _client;
        private readonly GithubPeerConfig _config;

        public GitHubRequestFetcher(GraphQLHttpClient client, GithubPeerConfig githubPeerConfig)
        {
            _client = client;
            _config = githubPeerConfig;
        }
    }
}
