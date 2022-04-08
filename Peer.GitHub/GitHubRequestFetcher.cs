using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
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
    public sealed class GitHubRequestFetcher : IPullRequestFetcher, IDisposable
    {
        private readonly GraphQLHttpClient _gqlClient;
        private readonly GitHubPeerConfig _config;
        private readonly AsyncLazy<string> _username;
        private readonly CancellationTokenSource _cts = new();
        private  CancellationTokenRegistration _registration;
        public GitHubRequestFetcher(
            GraphQLHttpClient client,
            GitHubPeerConfig gitHubPeerConfig)
        {
            _gqlClient = client;
            _config = gitHubPeerConfig;
            _username = new AsyncLazy<string>(() => GetUsername(_cts.Token));
        }

        public IAsyncEnumerable<PullRequest> GetPullRequestsAsync(CancellationToken token = default)
        {
            _registration = token.Register(() => _cts.Cancel());
            return GetPullRequestsImpl(token);
        }

        private async IAsyncEnumerable<PullRequest> GetPullRequestsImpl([EnumeratorCancellation] CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            var responses = new List<IAsyncEnumerable<PRSearch.PullRequest>>
                {
                    QueryGithubPullRequests(QueryType.Involves, token),
                    QueryGithubPullRequests(QueryType.TeamRequested, token)
                }.Merge();

            var deduplicated = responses.Distinct(x => x.Id);

            var prs = new Dictionary<string, PRSearch.PullRequest>();
            var prsWithMoreThreads = new List<PRSearch.PullRequest>();

            await foreach (var value in deduplicated)
            {
                if (value.ReviewThreads.PageInfo.HasNextPage)
                {
                    prsWithMoreThreads.Add(value);
                    prs[value.Id] = value;
                    continue;
                }

                yield return value.Into();
            }

            while (prsWithMoreThreads.Any())
            {
                var query = ThreadPageQuery(prsWithMoreThreads);

                var queryResponse = await RetryUntilCancelled(
                    async () => await _gqlClient.SendQueryAsync<Dictionary<string, PRSearch.PullRequest>>(query, token),
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

        private async IAsyncEnumerable<PRSearch.PullRequest> QueryGithubPullRequests(QueryType type, [EnumeratorCancellation] CancellationToken token)
        {
            var cursor = null as string;

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var response = await GetPullRequests(type, cursor, token);

                if (response.Errors != null)
                {
                    Console.WriteLine($"ERROR: {string.Join('\n', response.Errors.Select(x => x.Message))}");
                }

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
            }
        }

        private async Task<GraphQLResponse<GQL.SearchResult<PRSearch.Result>>> GetPullRequests(
            QueryType type,
            string? cursor, CancellationToken token)
        {
            var request = await GenerateRequest(type, cursor);

            return await RetryUntilCancelled(
                async () => await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(request, token),
                token);
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
            var viewerResponse = await RetryUntilCancelled(
                async () => await _gqlClient.SendQueryAsync<ViewerQuery.Result>(query, token),
                token);
            return viewerResponse.Data.Viewer.Login;
        }

        private static GraphQLHttpRequest ThreadPageQuery(IEnumerable<PRSearch.PullRequest> prs)
        {
            var query = ThreadQuery.Query.Generate(
                prs.Select(
                    pr => new ThreadQuery.QueryParams(pr.Id, pr.ReviewThreads.PageInfo.EndCursor)));

            return new GraphQLHttpRequest(query);
        }

        private static async Task<T> RetryUntilCancelled<T>(Func<Task<T>> fn, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    return await fn();
                }
                // Catching these exceptions works because we're only retrying queries, but it seems like something we
                // should specify at the callsite instead.
                catch (Exception ex) when (ex is GraphQLHttpRequestException || ex is HttpRequestException) { }
            }
        }

        public void Dispose()
        {
            _registration.Dispose();
        }
    }
}
