using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peer.Domain
{
    public interface IPullRequestFetcher
    {
        Task<IEnumerable<PullRequest>> GetPullRequestsAsync();
    }
}
