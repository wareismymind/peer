using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Peer.Domain;
using Peer.Domain.Exceptions;
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
            while (!token.IsCancellationRequested)
            {
                var responses = AsyncEnumerableEx.Merge(
                    QueryGithubPullRequests(QueryType.Involves, token),
                    QueryGithubPullRequests(QueryType.TeamRequested, token));

                var deduplicated = responses.Distinct(x => x.Id);
                
                var prs = new Dictionary<string, PRSearch.PullRequest>();
                var prsWithMoreThreads = new List<PRSearch.PullRequest>();

                await foreach (var value in deduplicated)
                {
                    if (value.ReviewThreads.PageInfo.HasNextPage)
                    {
                        prsWithMoreThreads.Add(value);
                        prs[value.Id] = value;
                    }

                    yield return value.Into();
                }

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
            }
        }

        private async IAsyncEnumerable<PRSearch.PullRequest> QueryGithubPullRequests(QueryType type,[EnumeratorCancellation] CancellationToken token)
        {
            var cursor = null as string;

            while (!token.IsCancellationRequested)
            {
                var response = await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await GenerateRequest(type, cursor), token);

                //CN: Probably don't wanna do this here. I mean we can but it doesn't make a ton of sense to result
                // it (even if we're sure we're done)
                var pageInfo = response.Data.Search.PageInfo;

                cursor = pageInfo.EndCursor;

                foreach (var pr in response.Data.Search.Nodes)
                {
                    yield return pr;
                }

                if (!pageInfo.HasNextPage)
                {
                    break;
                }

                //response.Data.Search.Nodes

                ////var deduplicated = responses.SelectMany(x => x.Data.Search.Nodes)
                ////    .DistinctBy(x => x.Id);

                //var prs = deduplicated.ToDictionary(pr => pr.Id);
                //var prsWithMoreThreads = prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

                //while (prsWithMoreThreads.Any())
                //{
                //    var queryResponse =
                //        await _gqlClient.SendQueryAsync<Dictionary<string, PRSearch.PullRequest>>(
                //            ThreadPageQuery(prsWithMoreThreads),
                //            token);

                //    var prThreadPages = queryResponse.Data.Values;

                //    foreach (var pr in prThreadPages)
                //    {
                //        prs[pr.Id].ReviewThreads.Nodes.AddRange(pr.ReviewThreads.Nodes);
                //    }

                //    prsWithMoreThreads =
                //        prThreadPages.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();
                //}

                //foreach (var value in prs.Values.Select(x => x.Into()))
                //{
                //    yield return value;
                //}

            }
        }

        private enum QueryType
        {
            Involves,
            TeamRequested
        }

        private async Task<GraphQLHttpRequest> GenerateRequest(QueryType type, string? endCursor = null)
        {
            var searchParams = new PRSearch.SearchParams(
                await _username,
                _config.Orgs,
                _config.ExcludedOrgs,
                _config.Count,
                endCursor);

            var queryString = type switch
            {
                QueryType.Involves => PRSearch.Search.GenerateInvolves(searchParams),
                QueryType.TeamRequested => PRSearch.Search.GenerateReviewRequested(searchParams),
                _ => throw new UnreachableException()
            };

            return new GraphQLHttpRequest(queryString);
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
