using System.Collections.Generic;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class Result
    {
        public int IssueCount { get; set; }
        public List<PullRequest> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
#nullable enable
}
