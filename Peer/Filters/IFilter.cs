using System.Collections.Generic;
using Peer.Domain;

namespace Peer.Filters
{
    public interface IFilter
    {
        IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests);
    }
}
