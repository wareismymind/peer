using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public interface IPullRequestService
    {
        Task<IEnumerable<PullRequest>> FetchAllPullRequests(CancellationToken token = default);
        Task<Result<PullRequest, FindError>> FindByPartial(PartialIdentifier partial, CancellationToken token = default);
    }
}
