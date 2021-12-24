using System.Collections.Generic;
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
        public Task<IAsyncEnumerable<PullRequest>> FetchAllPullRequests(CancellationToken token = default)
        {
            var prIterator = _fetchers.ToAsyncEnumerable().SelectManyAwait(async x => await x.GetPullRequestsAsync(token));
            return Task.FromResult(prIterator);
        }

        public async Task<Result<PullRequest, FindError>> FindSingleByPartial(PartialIdentifier partial, CancellationToken token = default)
        {
            Validators.ArgIsNotNull(partial);

            var prs = await FetchAllPullRequests(token);

            var matches = await prs.Where(x => x.Identifier.IsMatch(partial)).ToListAsync(token);

            return matches.Count switch
            {
                0 => FindError.NotFound,
                1 => matches.First(),
                _ => FindError.AmbiguousMatch,
            };
        }
    }
}
