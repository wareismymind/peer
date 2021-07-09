using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain.Models;

namespace Peer.Domain
{
    public interface IPullRequestSource
    {
        Task<IEnumerable<PullRequest>> FetchPullRequests();
    }
}
