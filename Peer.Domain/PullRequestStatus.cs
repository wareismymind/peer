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
        FixesRequired,
        ReadyToMerge,
        Merged,
        Stale
    }
}
