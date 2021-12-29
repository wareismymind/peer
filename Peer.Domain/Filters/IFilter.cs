using System.Collections.Generic;

namespace Peer.Domain.Filters
{
    public interface IFilter
    {
        IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests);
    }
}
