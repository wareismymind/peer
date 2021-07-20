using System.Collections.Generic;
using System.Threading.Tasks;
using Peer.Domain.Models;

namespace Peer.Domain
{
    public interface IPullRequestSource
    {
        Task<IEnumerable<PeerPullRequest>> FetchPullRequests();
    }
}
