using System.Collections.Generic;

namespace Peer.Domain
{
    public interface IListFormatter
    {
        IEnumerable<string> FormatLines(IEnumerable<PullRequest> pullRequests);
    }
}
