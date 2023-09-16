using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain;

namespace Peer.Filters
{
    public class ActionFilter : IFilter
    {
        private readonly Func<PullRequest, bool> _func;
        private readonly bool _negated;

        public ActionFilter(Func<PullRequest, bool> func, bool negated)
        {
            _func = func;
            _negated = negated;
        }

        public IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests)
        {
            return pullRequests.Where(x => _func(x) ^ _negated);
        }
    }
}
