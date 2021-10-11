using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using Peer.Domain;

namespace Peer
{
    public class GitHubRequestFetcher: IPullRequestFetcher
    {
        // involves

        private const string _initialQueryFmt = @"{{
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
        }}";

        private const string _pageQueryItemFmt = @"{0}: node(id: ""{0}"") {{
            ... on PullRequest {{
                reviewThreads(first: 100, after: ""{1}"") {{
                    nodes {{ isResolved }}
                    pageInfo {{ endCursor, hasNextPage }}
                }}
            }}
        }}";

        private readonly GraphQLHttpClient _gqlClient;
        private readonly string _usersClause;

        public GitHubRequestFetcher(GraphQLHttpClient gqlClient, IEnumerable<string> users)
        {
            _gqlClient = gqlClient ?? throw new ArgumentNullException(nameof(gqlClient));

            var usersList = users?.ToList() ?? throw new ArgumentNullException(nameof(users));
            if (usersList.Count == 0) throw new ArgumentException("Must not be empty", nameof(users));
            _usersClause = string.Join(' ', usersList.Select(u => $"user:{u}"));
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync()
        {
            var query = string.Format(_initialQueryFmt, _usersClause);
            var request = new GraphQLHttpRequest(query);
            var response = await _gqlClient.SendQueryAsync<GqlQueryResult>(request);

            // todo: Count total/active comments with paging
            return response.Data.Search.Nodes
                .Select(pr => {
                    // todo: Real status
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
    }
}
