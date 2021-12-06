namespace Peer.Domain
{
    public enum CheckResult
    {
        Unknown,
        Success,
        Failure,
        Neutral,
        Skipped,
        Cancelled,
        Stale, 
        Timeout,
        ActionRequired,
        Fire, 
    }
}
