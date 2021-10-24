using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int PRSearchLimit = 20;

        private readonly GraphQLHttpClient _gqlClient;
        private readonly GitHubPeerConfig _config;
        private readonly AsyncLazy<string> _username;
        private readonly AsyncLazy<GraphQLHttpRequest> _involvesRequest;
        private readonly AsyncLazy<GraphQLHttpRequest> _reviewRequestedRequest;

        public GitHubRequestFetcher(GraphQLHttpClient client, GitHubPeerConfig gitHubPeerConfig)
        {
            _gqlClient = client;
            _config = gitHubPeerConfig;
            _username = new AsyncLazy<string>(GetUsername);
            _involvesRequest = new AsyncLazy<GraphQLHttpRequest>(() => GenerateInvolvesRequest());
            _reviewRequestedRequest = new AsyncLazy<GraphQLHttpRequest>(() => GenerateReviewRequestedRequest());
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync()
        {
            var involvesResponse =
                await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await _involvesRequest);

            var reviewRequested = await _gqlClient.SendQueryAsync<GQL.SearchResult<PRSearch.Result>>(await _reviewRequestedRequest);

            var deduplicated = involvesResponse.Data.Search.Nodes
                .Concat(reviewRequested.Data.Search.Nodes)
                .DistinctBy(x => x.Id);

            // todo: Handle errors.

            var prs = deduplicated.ToDictionary(pr => pr.Id);

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

        private async Task<GraphQLHttpRequest> GenerateInvolvesRequest()
        {
            var username = string.IsNullOrEmpty(_config.Username)
                ? await GetUsername()
                : _config.Username;

            var searchParams = new PRSearch.SearchParams(
                username, _config.Orgs, _config.ExcludedOrgs, PRSearchLimit);

            return new GraphQLHttpRequest(PRSearch.Search.GenerateInvolves(searchParams), default);
        }

        private async Task<GraphQLHttpRequest> GenerateReviewRequestedRequest()
        {
            var username = string.IsNullOrEmpty(_config.Username)
                ? await GetUsername()
                : _config.Username;

            var searchParams = new PRSearch.SearchParams(
                username, _config.Orgs, _config.ExcludedOrgs, PRSearchLimit);

            return new GraphQLHttpRequest(PRSearch.Search.GenerateReviewRequested(searchParams), default);
        }

        private async  Task<string> GetUsername()
        {
            return string.IsNullOrEmpty(_config.Username)
                ? await QueryUsername()
                : _config.Username;
        }

        private async Task<string> QueryUsername()
        {
            var query = new GraphQLHttpRequest(ViewerQuery.Query.Generate());
            var viewerResponse = await _gqlClient.SendQueryAsync<ViewerQuery.Result>(query);
            return viewerResponse.Data.Viewer.Login;
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
