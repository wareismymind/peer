using System.Collections.Generic;

namespace Peer.Domain
{
    public interface IPullRequestFormatter
    {
        public IEnumerable<string> FormatLines(IEnumerable<PullRequest> pullRequests);
    }
}
