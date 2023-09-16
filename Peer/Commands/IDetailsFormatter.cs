using System.Collections.Generic;
using Peer.Domain;

namespace Peer.Commands
{
    public interface IDetailsFormatter
    {
        IList<string> Format(PullRequest pullRequest);
    }
}
