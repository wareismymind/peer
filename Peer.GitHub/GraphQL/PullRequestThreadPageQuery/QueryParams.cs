
namespace Peer.GitHub.GraphQL.PullRequestThreadPageQuery
{
    public class QueryParams
    {
        public string PullRequestId { get; }
        public string EndCursor { get; }

        public QueryParams(string pullRequestId, string endCursor)
        {
            PullRequestId = pullRequestId;
            EndCursor = endCursor;
        }
    }
}
