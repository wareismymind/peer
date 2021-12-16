namespace Peer.Domain
{
    public enum CheckStatus
    {
        Requested,
        Queued,
        PendingExecution,
        InProgress,
        Waiting,
        Completed
    }
}
