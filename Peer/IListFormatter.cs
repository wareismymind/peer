using System.Collections.Generic;
using Peer.Domain;

namespace Peer
{
    public interface IListFormatter
    {
        IEnumerable<string> FormatLines(IEnumerable<PullRequest> pullRequests);
    }
}
