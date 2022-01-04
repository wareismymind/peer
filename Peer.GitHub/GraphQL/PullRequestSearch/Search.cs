using System.Linq;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
    public static class Search
    {
        public static string GenerateReviewRequested(SearchParams search)
        {
            var reviewRequestedClause = $"review-requested:{search.Subject}";
            var orgsClauses = string.Join(' ', search.Orgs.Select(o => $"org:{o}"));
            var excludedOrgsClauses = string.Join(' ', search.ExcludedOrgs.Select(o => $"-org:{o}"));
            var searchTerms = string.Join(' ', reviewRequestedClause, orgsClauses, excludedOrgsClauses);

            return GenerateQuery(searchTerms, search.PageSize, search.After);
        }

        public static string GenerateInvolves(SearchParams search)
        {
            var involvesClause = $"involves:{search.Subject}";
            var orgsClauses = string.Join(' ', search.Orgs.Select(o => $"org:{o}"));
            var excludedOrgsClauses = string.Join(' ', search.ExcludedOrgs.Select(o => $"-org:{o}"));

            var searchTerms = $"{involvesClause} {orgsClauses} {excludedOrgsClauses}";
            return GenerateQuery(searchTerms, search.PageSize, search.After);
        }

        private static string GenerateQuery(string searchTerms, int pageSize, string? endCursor = null)
        {
            var afterValue = endCursor == null ? string.Empty : $@"after: ""{endCursor}""";
            return $@"
                {{
                    search(query: ""is:pr is:open archived:false {searchTerms}"", type: ISSUE, first: {pageSize} {afterValue}) {{
                        issueCount
                        pageInfo {{ endCursor, hasNextPage }}
                        nodes {{
                            {_prValueQuery}
                        }}
                    }}
                }}";
        }

        private const string _prValueQuery = @"
                            ... on PullRequest {
                                id
                                number
                                url
                                title
                                createdAt
                                body
                                baseRefName
                                headRefName
                                reviewThreads(first: 100) {
                                    nodes { isResolved }
                                    pageInfo { hasNextPage, endCursor }
                                }
                                isDraft
                                state
                                mergeable
                                reviewDecision
                                author {
                                    login
                                }
                                baseRepository {
                                    name
                                    owner { login }
                                }
#THIS IS THE STUFF FOR THE CHECKS
                                commits(last: 1) {
                                  nodes {
                                    commit {
                                      statusCheckRollup { state }
                                      checkSuites(first: 20) {
                                        nodes {
                                          checkRuns(first: 20) {
                                            nodes {
                                              name
                                              conclusion
                                              status
                                              url
                                          }
                                        }
                                      }
                                    }
                                  }
                                }
                              }
#THIS IS THE END OF THE STUFF FOR THE CHECKS
                            }";
    }
}
