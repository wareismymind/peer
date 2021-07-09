using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Domain.Models;

namespace Peer.Connectors
{
    public class GithubPullRequestSource : IPullRequestSource
    {
        public Task<IEnumerable<PullRequest>> FetchPullRequests() //???????? account?
        {
            throw new NotImplementedException();
        }
    }
}
