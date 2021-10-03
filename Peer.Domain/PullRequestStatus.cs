namespace Peer.Domain
{
    public enum PullRequestStatus
    {
        Unknown,
        Draft,
        Conflict,
        FailedChecks,
        ActionsQueued,
        ActionsPending,
        AwaitingReview,
        FixesRequested,
        ReadyToMerge,
        Merged,
        Stale
    }
}
