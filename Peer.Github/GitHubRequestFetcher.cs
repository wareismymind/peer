using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Peer.Domain;

namespace Peer.GitHub
{
    public class GitHubRequestFetcher : IPullRequestFetcher
    {
        private readonly GraphQLHttpClient _gqlClient;
        private readonly GithubPeerConfig _config;
        private readonly GraphQLHttpRequest _searchRequest;

        public GitHubRequestFetcher(GraphQLHttpClient client, GithubPeerConfig githubPeerConfig)
        {
            _gqlClient = client;
            _config = githubPeerConfig;

            var orgsClauses = string.Join(' ', _config.Orgs.Select(o => $"org:{o}"));
            var involvesClause = $"involves:{_config.Username}";
            _searchRequest = new GraphQLHttpRequest(GenerateInitialSearch($"{involvesClause} {orgsClauses}"));
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync()
        {
            var searchResponse = await _gqlClient.SendQueryAsync<GqlQueryResult>(_searchRequest);

            // todo: Handle errors.

            var prs = searchResponse.Data.Search.Nodes.ToDictionary(pr => pr.Id);

            var prsWithMoreThreads =
                prs.Values.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();

            while (prsWithMoreThreads.Any())
            {
                var query = GenerateThreadPageQuery(prsWithMoreThreads);
                var queryRequest = new GraphQLHttpRequest(query);
                var queryResponse =
                    await _gqlClient.SendQueryAsync<Dictionary<string, GqlPullRequest>>(
                        queryRequest);

                var prThreadPages = queryResponse.Data.Values;

                foreach (var pr in prThreadPages)
                {
                    prs[pr.Id].ReviewThreads.Nodes.AddRange(pr.ReviewThreads.Nodes);
                }

                prsWithMoreThreads =
                    prThreadPages.Where(pr => pr.ReviewThreads.PageInfo.HasNextPage).ToList();
            }

            return prs.Values
                .Select(pr => {
                    // todo: Calculate status.
                    var status = PullRequestStatus.ReadyToMerge;
                    var totalComments = pr.ReviewThreads.Nodes.Count;
                    var activeComments = pr.ReviewThreads.Nodes.Count(t => !t.IsResolved);
                    return new PullRequest(
                        pr.Number.ToString(),
                        pr.Url,
                        new Descriptor(pr.Title, pr.Body ?? string.Empty),
                        new State(status, totalComments, activeComments),
                        new GitInfo(pr.HeadRefName, pr.BaseRefName));
                });
        }

        private static string GenerateInitialSearch(string searchTerms)
        {
            return string.Format(
                @"{{
                    search(query: ""is:pr is:open archived:false {0}"", type: ISSUE, first: 20) {{
                        issueCount
                        nodes {{
                            ... on PullRequest {{
                                id
                                number
                                url
                                title
                                body
                                baseRefName
                                headRefName
                                reviewThreads(first: 100) {{
                                    nodes {{ isResolved }}
                                    pageInfo {{ hasNextPage, endCursor }}
                                }}
                            }}
                        }}
                        pageInfo {{ endCursor }}
                    }}
                }}",
                searchTerms);
        }

        private static string GenerateThreadPageQuery(IEnumerable<GqlPullRequest> prs)
        {
            var queryItems = prs.Select(GenerateThreadPageQueryItem);
            return $"query {{ {string.Join('\n', queryItems)} }}";
        }

        private static string GenerateThreadPageQueryItem(GqlPullRequest pr)
        {
            return string.Format(
                @"{0}: node(id: ""{0}"") {{
                    ... on PullRequest {{
                        id
                        reviewThreads(first: 100, after: ""{1}"") {{
                            nodes {{ isResolved }}
                            pageInfo {{ endCursor, hasNextPage }}
                        }}
                    }}
                }}",
                pr.Id,
                pr.ReviewThreads.PageInfo.EndCursor);
        }

#nullable disable
        public class GqlQueryResult
        {
            public GqlSearchResult Search { get; set; }
        }

        public class GqlSearchResult
        {
            public int IssueCount { get; set; }
            public List<GqlPullRequest> Nodes { get; set; }
            public GqlPageInfo PageInfo { get; set; }
        }

        public class GqlPullRequest
        {
            public string Id { get; set; }
            public int Number { get; set; }
            public Uri Url { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public string BaseRefName { get; set; }
            public string HeadRefName { get; set; }
            public GqlReviewThreads ReviewThreads { get; set; }
        }

        public class GqlReviewThreads
        {
            public List<GqlReviewThread> Nodes { get; set; }
            public GqlPageInfo PageInfo { get; set; }
        }

        public class GqlReviewThread
        {
            public bool IsResolved { get; set; }
        }

        public class GqlPageInfo
        {
            public bool HasNextPage { get; set; }
            public string EndCursor { get; set; }
            public bool HasPreviousPage { get; set; }
            public string StartCursor { get; set; }
        }
#nullable enable
    }
}
