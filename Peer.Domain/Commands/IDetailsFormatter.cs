using System.Collections.Generic;

namespace Peer.Domain.Commands
{
    public interface IDetailsFormatter
    {
        IList<string> Format(PullRequest pullRequest);
    }
}
