using System.Collections.Generic;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class ReviewThreads 
        : NodeList<ReviewThread>
    {
        public PageInfo PageInfo { get; set; }
    }
#nullable enable
}
