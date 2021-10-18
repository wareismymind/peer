using System.Collections.Generic;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class ReviewThreads
    {
        public List<ReviewThread> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
#nullable enable
}
