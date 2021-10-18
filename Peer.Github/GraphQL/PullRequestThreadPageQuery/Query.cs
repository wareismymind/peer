using System.Collections.Generic;
using System.Linq;
using Peer.Domain;

namespace Peer.GitHub.GraphQL.PullRequestThreadPageQuery
{
    public static class Query
    {
        public static string Generate(IEnumerable<QueryParams> prs)
        {
            var queryItems = prs.Select(GenerateItem);
            return $"query {{ {string.Join('\n', queryItems)} }}";
        }

        private static string GenerateItem(QueryParams query)
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
                query.PullRequestId,
                query.EndCursor);
        }
    }
}
