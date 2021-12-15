﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class PullRequestService : IPullRequestService
    {
        private readonly List<IPullRequestFetcher> _fetchers;

        public PullRequestService(IEnumerable<IPullRequestFetcher> fetchers)
        {
            _fetchers = fetchers.ToList();
        }

        //TODO(cn): AsyncEnumerable
        public async Task<IEnumerable<PullRequest>> FetchAllPullRequests(CancellationToken token = default)
        {
            var tasks = _fetchers.Select(async x => await x.GetPullRequestsAsync(token));
            var prs = await Task.WhenAll(tasks);
            var combined = prs.Aggregate((x, y) => x.Concat(y));
            return combined;
        }

        public async Task<Result<PullRequest, FindError>> FindByPartial(PartialIdentifier partial, CancellationToken token = default)
        {
            Validators.ArgIsNotNull(partial);

            var prs = await FetchAllPullRequests(token);

            var matches = prs.Where(x => x.Identifier.IsMatch(partial)).ToList();

            return matches.Count switch
            {
                0 => FindError.NotFound,
                1 => matches.First(),
                _ => FindError.AmbiguousMatch,
            };
        }
    }
}