﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain;

public interface IPullRequestService
{
    IAsyncEnumerable<PullRequest> FetchAllPullRequests(CancellationToken token = default);
    Task<Result<PullRequest, FindError>> FindSingleByPartial(PartialIdentifier partial, CancellationToken token = default);
}

public enum FindError
{
    AmbiguousMatch,
    NotFound
}
