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
        private readonly GraphQLHttpClient _gqlClient;
        private readonly GitHubPeerConfig _config;
        private readonly AsyncLazy<string> _username;
        private readonly AsyncLazy<GraphQLHttpRequest> _involvesRequest;
        private readonly AsyncLazy<GraphQLHttpRequest> _reviewRequestedRequest;
        private readonly CancellationTokenSource _cts = new();

        public GitHubRequestFetcher(GraphQLHttpClient client, GitHubPeerConfig gitHubPeerConfig)
        {
            _gqlClient = client;
            _config = gitHubPeerConfig;
            _username = new AsyncLazy<string>(() => GetUsername(_cts.Token));
            _involvesRequest = new AsyncLazy<GraphQLHttpRequest>(() => GenerateInvolvesRequest());
            _reviewRequestedRequest = new AsyncLazy<GraphQLHttpRequest>(() => GenerateReviewRequestedRequest());
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync(CancellationToken token = default)
        {
            using var registration = token.Register(() => _cts.Cancel());

            var involvesResponse =
                await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await _involvesRequest, token);

            var reviewRequestedResponse = await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await _reviewRequestedRequest, token);

            var deduplicated = involvesResponse.Data.Search.Nodes
                .Concat(reviewRequestedResponse.Data.Search.Nodes)
                .DistinctBy(x => x.Id);

            // todo: Handle errors.

            var prs = deduplicated.ToDictionary(pr => pr.Id);

            var prsWithMoreThreads =
                prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

            while (prsWithMoreThreads.Any())
            {
                var queryResponse =
                    await _gqlClient.SendQueryAsync<Dictionary<string, PRSearch.PullRequest>>(
                        ThreadPageQuery(prsWithMoreThreads),
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

        private async Task<GraphQLHttpRequest> GenerateInvolvesRequest()
        {
            var searchParams = new PRSearch.SearchParams(
                await _username,
                _config.Orgs,
                _config.ExcludedOrgs,
                _config.Count);

            return new GraphQLHttpRequest(PRSearch.Search.GenerateInvolves(searchParams));
        }

        private async Task<GraphQLHttpRequest> GenerateReviewRequestedRequest()
        {
            var searchParams = new PRSearch.SearchParams(
                await _username,
                _config.Orgs,
                _config.ExcludedOrgs,
                _config.Count);

            return new GraphQLHttpRequest(PRSearch.Search.GenerateReviewRequested(searchParams));
        }

        private async Task<string> GetUsername(CancellationToken token)
        {
            return string.IsNullOrEmpty(_config.Username)
                ? await QueryUsername(token)
                : _config.Username;
        }

        private async Task<string> QueryUsername(CancellationToken token)
        {
            var query = new GraphQLHttpRequest(ViewerQuery.Query.Generate(), token);
            var viewerResponse = await _gqlClient.SendQueryAsync<ViewerQuery.Result>(query, token);
            return viewerResponse.Data.Viewer.Login;
        }

        private static GraphQLHttpRequest ThreadPageQuery(IEnumerable<PRSearch.PullRequest> prs)
        {
            var query = ThreadQuery.Query.Generate(
                   prs.Select(
                       pr => new ThreadQuery.QueryParams(pr.Id, pr.ReviewThreads.PageInfo.EndCursor)));

            return new GraphQLHttpRequest(query);
        }
    }
}
