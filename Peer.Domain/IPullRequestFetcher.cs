using System.Collections.Generic;
using System.Threading;

namespace Peer.Domain
{
    public interface IPullRequestFetcher
    {
        IAsyncEnumerable<PullRequest> GetPullRequestsAsync(CancellationToken token = default);
    }
}
