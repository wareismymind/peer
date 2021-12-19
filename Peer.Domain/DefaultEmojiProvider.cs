using Peer.Domain.Exceptions;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class DefaultEmojiProvider : ISymbolProvider, ICheckSymbolProvider
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

        public Maybe<string> GetSymbol(CheckStatus status, CheckResult result)
        {
            var check = new { Status = status, Result = result };
            return check switch
            {
                { Status: CheckStatus.InProgress, Result: CheckResult.Unknown } => "\uD83D\uDD35", //Large Blue Circle
                { Status: CheckStatus.Completed, Result: CheckResult.Success } => "\uD83D\uDFE2", //Large Green Circle
                { Status: CheckStatus.Completed, Result: CheckResult.Failure } => "\uD83D\uDD34", //Large Red Circle
                { Status: CheckStatus.Completed, Result: CheckResult.Timeout } => "\uD83D\uDC22", //Turtle
                { Status: CheckStatus.Completed, Result: CheckResult.Skipped } => "\uD83E\uDEA2", //Knot (Kinda like a skipping rope? Really reaching)
                { Status: CheckStatus.Completed, Result: CheckResult.Neutral } => "\uD83E\uDD37", //Shrug
                _ => Maybe.None
            };
        }
    }
}
