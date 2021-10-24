using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Peer.Domain;
using Peer.Utils;
using GQL = Peer.GitHub.GraphQL;
using PRSearch = Peer.GitHub.GraphQL.PullRequestSearch;
using ThreadQuery = Peer.GitHub.GraphQL.PullRequestThreadPageQuery;
using ViewerQuery = Peer.GitHub.GraphQL.ViewerQuery;

namespace Peer.GitHub
{
    public class GitHubRequestFetcher : IPullRequestFetcher
    {
        private const int _prSearchLimit = 20;

        private readonly GraphQLHttpClient _gqlClient;
        private readonly GitHubPeerConfig _config;
        private readonly AsyncLazy<GraphQLHttpRequest> _searchRequest;
        private readonly CancellationTokenSource _cts = new();

        public GitHubRequestFetcher(GraphQLHttpClient client, GitHubPeerConfig gitHubPeerConfig)
        {
            _gqlClient = client;
            _config = gitHubPeerConfig;
            _searchRequest = new AsyncLazy<GraphQLHttpRequest>(() => GenerateSearchRequest(_cts.Token));
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync(CancellationToken token = default)
        {
            using var registration = token.Register(() => _cts.Cancel());

            var searchResponse =
                await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await _searchRequest, token);

            // todo: Handle errors.

            var prs = searchResponse.Data.Search.Nodes.ToDictionary(pr => pr.Id);

            var prsWithMoreThreads =
                prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

            while (prsWithMoreThreads.Any())
            {
                var queryResponse =
                    await _gqlClient.SendQueryAsync<Dictionary<string, PRSearch.PullRequest>>(
                        ThreadPageQuery(prsWithMoreThreads, token),
                        token);

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

        private async Task<GraphQLHttpRequest> GenerateSearchRequest(CancellationToken token = default)
        {
            var username = string.IsNullOrEmpty(_config.Username)
                ? await GetUsername(token)
                : _config.Username;

            var searchParams = new PRSearch.SearchParams(
                username, _config.Orgs, _config.ExcludedOrgs, _prSearchLimit);

            return new GraphQLHttpRequest(PRSearch.Search.Generate(searchParams), default);
        }

        private async Task<string> GetUsername(CancellationToken token = default)
        {
            var query = new GraphQLHttpRequest(ViewerQuery.Query.Generate(), token);
            var viewerResponse = await _gqlClient.SendQueryAsync<ViewerQuery.Result>(query, token);
            return viewerResponse.Data.Viewer.Login;
        }

        private static GraphQLHttpRequest ThreadPageQuery(IEnumerable<PRSearch.PullRequest> prs, CancellationToken token = default)
        {
            var query = ThreadQuery.Query.Generate(
                   prs.Select(
                       pr => new ThreadQuery.QueryParams(pr.Id, pr.ReviewThreads.PageInfo.EndCursor)));

            return new GraphQLHttpRequest(query, token);
        }
    }
}
