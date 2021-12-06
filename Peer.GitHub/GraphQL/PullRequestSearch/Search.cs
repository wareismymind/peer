﻿using System.Linq;

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

            return GenerateQuery(searchTerms, search.PageSize);
        }

        public static string GenerateInvolves(SearchParams search)
        {
            var involvesClause = $"involves:{search.Subject}";
            var orgsClauses = string.Join(' ', search.Orgs.Select(o => $"org:{o}"));
            var excludedOrgsClauses = string.Join(' ', search.ExcludedOrgs.Select(o => $"-org:{o}"));

            var searchTerms = $"{involvesClause} {orgsClauses} {excludedOrgsClauses}";
            return GenerateQuery(searchTerms, search.PageSize);
        }

        private static string GenerateQuery(string searchTerms, int pageSize)
        {
            return $@"{{
                    search(query: ""is:pr is:open archived:false {searchTerms}"", type: ISSUE, first: {pageSize}) {{
                        issueCount
                        nodes {{
                            ... on PullRequest {{
                                id
                                number
                                url
                                title
                                createdAt
                                body
                                baseRefName
                                headRefName
                                reviewThreads(first: 100) {{
                                    nodes {{ isResolved }}
                                    pageInfo {{ hasNextPage, endCursor }}
                                }}
                                isDraft
                                state
                                mergeable
                                reviewDecision
                                baseRepository {{
                                    name
                                    owner {{ login }}
                                }}
#THIS IS THE STUFF FOR THE CHECKS AND CRAP
                                commits(last: 1) {{
                                  nodes {{
                                    commit {{
                                      checkSuites(first: 100) {{
                                        nodes {{
                                          checkRuns(first: 100) {{
                                            nodes {{
                                              name
                                              conclusion
                                              status
                                              url
                                          }}
                                        }}
                                      }}
                                    }}
                                  }}
                                }}
                              }}
#THIS IS THE END OF THE STUFF FOR THE CHECKS AND CRAP
                            }}
                        }}
                        pageInfo {{ endCursor }}
                    }}
                }}";
        }
    }
}
