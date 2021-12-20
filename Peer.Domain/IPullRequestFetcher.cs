using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Peer.Domain
{
    public interface IPullRequestFetcher
    {
        Task<IAsyncEnumerable<PullRequest>> GetPullRequestsAsync(CancellationToken token = default);
    }
}
