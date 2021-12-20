using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly CancellationTokenSource _cts = new();

        public GitHubRequestFetcher(GraphQLHttpClient client, GitHubPeerConfig gitHubPeerConfig)
        {
            _gqlClient = client;
            _config = gitHubPeerConfig;
            _username = new AsyncLazy<string>(() => GetUsername(_cts.Token));
        }

        public Task<IAsyncEnumerable<PullRequest>> GetPullRequestsAsync(CancellationToken token = default)
        {
            using var registration = token.Register(() => _cts.Cancel());
            return Task.FromResult(GetPullRequestsImpl(token));
        }

        private async IAsyncEnumerable<PullRequest> GetPullRequestsImpl([EnumeratorCancellation] CancellationToken token)
        {
            var involvesCursor = null as string;
            var requestedCursor = null as string;

            while (!token.IsCancellationRequested)
            {
                var involvesResponse = _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await GenerateInvolvesRequest(involvesCursor), token);
                var reviewRequestedResponse = _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await GenerateReviewRequestedRequest(requestedCursor), token);

                var responses = await Task.WhenAll(involvesResponse, reviewRequestedResponse);

                //CN: Probably don't wanna do this here. I mean we can but it doesn't make a ton of sense to result
                // it (even if we're sure we're done)
                var involvesPageInfo = involvesResponse.Result.Data.Search.PageInfo;
                var requestedPageInfo = reviewRequestedResponse.Result.Data.Search.PageInfo;

                involvesCursor = involvesPageInfo.EndCursor;
                requestedCursor = requestedPageInfo.EndCursor;

                var deduplicated = responses.SelectMany(x => x.Data.Search.Nodes)
                    .DistinctBy(x => x.Id);

                var prs = deduplicated.ToDictionary(pr => pr.Id);
                var prsWithMoreThreads = prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

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

                foreach (var value in prs.Values.Select(x => x.Into()))
                {
                    yield return value;
                }

                if (!involvesPageInfo.HasNextPage && !requestedPageInfo.HasNextPage)
                {
                    break;
                }
            }
        }

        private async Task<GraphQLHttpRequest> GenerateInvolvesRequest(string? endCursor = null)
        {
            var searchParams = new PRSearch.SearchParams(
                await _username,
                _config.Orgs,
                _config.ExcludedOrgs,
                _config.Count,
                endCursor);

            return new GraphQLHttpRequest(PRSearch.Search.GenerateInvolves(searchParams));
        }

        private async Task<GraphQLHttpRequest> GenerateReviewRequestedRequest(string? endCursor = null)
        {
            var searchParams = new PRSearch.SearchParams(
                await _username,
                _config.Orgs,
                _config.ExcludedOrgs,
                _config.Count,
                endCursor);

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
