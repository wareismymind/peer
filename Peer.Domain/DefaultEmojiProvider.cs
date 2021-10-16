using Peer.Domain.Exceptions;

namespace Peer.Domain;

public class DefaultEmojiProvider : ISymbolProvider
{
    public string GetSymbol(PullRequestStatus status)
    {
        return status switch
        {
            PullRequestStatus.Unknown => "\u2754", //white question mark
            PullRequestStatus.Conflict => "\u2694\uFE0F", //Crossed swords with variant modifier (emoji)
            PullRequestStatus.Draft => "\uD83D\uDCC3", //Page with curl
            PullRequestStatus.ActionsPending => "\u231B", //Hourglass not done 
            PullRequestStatus.ActionsQueued => "\uD83D\uDCA4", //Zzz (sleeping symbol)
            PullRequestStatus.Stale => "\uD83C\uDF5E", //bread
            PullRequestStatus.FailedChecks => "\uD83D\uDD25", //fire
            PullRequestStatus.AwaitingReview => "\uD83D\uDEA9", //triangle flag (usually red)
            PullRequestStatus.FixesRequested => "\u274C", //cross mark (usually red)
            PullRequestStatus.ReadyToMerge => "\u2714\uFE0F", //heavy check mark with variant modifier (emoji)
            PullRequestStatus.Merged => "\uD83C\uDF8A", // confetti ball
            _ => throw new UnreachableException()
        };
    }
}
