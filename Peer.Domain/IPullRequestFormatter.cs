using System.Collections.Generic;

namespace Peer.Domain
{
    public interface IPullRequestFormatter
    {
        IEnumerable<string> FormatLines(IEnumerable<PullRequest> pullRequests);
    }
}
