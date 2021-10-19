using System.Linq;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
    public static class Search
    {
        public static string Generate(SearchParams search)
        {
            var involvesClause = $"involves:{search.Involves}";
            var orgsClauses = string.Join(' ', search.Orgs.Select(o => $"org:{o}"));
            var excludedOrgsClauses = string.Join(' ', search.ExcludedOrgs.Select(o => $"-org:{o}"));

            var searchTerms = $"{involvesClause} {orgsClauses} {excludedOrgsClauses}";

            return $@"{{
                    search(query: ""is:pr is:open archived:false {searchTerms}"", type: ISSUE, first: {search.PageSize}) {{
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
                                baseRepository {{
                                    name
                                    owner {{ login }}
                                }}
                            }}
                        }}
                        pageInfo {{ endCursor }}
                    }}
                }}";
        }
    }
}
