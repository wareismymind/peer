using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Peer.Domain;
using GQL = Peer.GitHub.GraphQL;
using PRSearch = Peer.GitHub.GraphQL.PullRequestSearch;
using ThreadQuery = Peer.GitHub.GraphQL.PullRequestThreadPageQuery;

namespace Peer.GitHub
{
    public class GitHubRequestFetcher : IPullRequestFetcher
    {
        private const int PRSearchLimit = 20;

        private readonly GraphQLHttpClient _gqlClient;
        private readonly GithubPeerConfig _config;
        private readonly GraphQLHttpRequest _searchRequest;

        public GitHubRequestFetcher(GraphQLHttpClient client, GithubPeerConfig githubPeerConfig)
        {
            _gqlClient = client;
            _config = githubPeerConfig;

            // todo: Get username via GraphQL query if it's not set in config.

            _searchRequest = new GraphQLHttpRequest(
                PRSearch.Search.Generate(_config.Username!, _config.Orgs, _config.ExcludedOrgs, PRSearchLimit));
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync()
        {
            var searchResponse = await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(_searchRequest);

            // todo: Handle errors.

            var prs = searchResponse.Data.Search.Nodes.ToDictionary(pr => pr.Id);

            var prsWithMoreThreads =
                prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

            while (prsWithMoreThreads.Any())
            {
                var queryResponse =
                    await _gqlClient.SendQueryAsync<Dictionary<string, PRSearch.PullRequest>>(
                        ThreadPageQuery(prsWithMoreThreads));

                var prThreadPages = queryResponse.Data.Values;

                foreach (var pr in prThreadPages)
                {
                    prs[pr.Id].ReviewThreads.Nodes.AddRange(pr.ReviewThreads.Nodes);
                }

                prsWithMoreThreads =
                    prThreadPages.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();
            }

            return prs.Values.Select(pr => pr.Into());
        }

        private GraphQLHttpRequest ThreadPageQuery(IEnumerable<PRSearch.PullRequest> prs)
        {
            var query = ThreadQuery.Query.Generate(
                   prs.Select(
                       pr => new ThreadQuery.QueryParams(pr.Id, pr.ReviewThreads.PageInfo.EndCursor)));

            return new GraphQLHttpRequest(query);
        }
    }
}
