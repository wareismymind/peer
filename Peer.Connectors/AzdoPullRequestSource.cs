using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer.Connectors
{
    class AzdoPullRequestSource : IPullRequestSource
    {
        private readonly ConfigModel _config;
        private static IClient<AzdoClient> _user;

        public AzdoPullRequestSource(ConfigModel config, IClient<AzdoClient> user)
        {
            _config = config;
            _user = user;
        }

        // TODO: implement the method to fetch the PRs
        public Task<IEnumerable<PeerPullRequest>> FetchPullRequests()
        {
            throw new NotImplementedException();
        }
    }
}
