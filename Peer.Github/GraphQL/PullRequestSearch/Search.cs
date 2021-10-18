using System.Collections.Generic;
using System.Linq;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
    public static class Search
    {
        public static string Generate(
            string involves,
            IEnumerable<string> orgs,
            IEnumerable<string> excludedOrgs,
            int first)
        {
            var involvesClause = $"involves:{involves}";
            var orgsClauses = string.Join(' ', orgs.Select(o => $"org:{o}"));
            var excludedOrgsClauses = string.Join(' ', excludedOrgs.Select(o => $"!org:{o}"));

            var searchTerms = $"{involvesClause} {orgsClauses} {excludedOrgsClauses}";

            return $@"{{
                    search(query: ""is:pr is:open archived:false {searchTerms}"", type: ISSUE, first: {first}) {{
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
        }
    }
}
