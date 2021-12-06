using System.Collections.Generic;
using System.Linq;

namespace Peer.Domain.Commands
{
    //TODO:CN -- Ansi term code support
    public class DetailsFormatter : IDetailsFormatter
    {
        public IList<string> Format(PullRequest pullRequest)
        {
            //TODO: More here - wrapping and splitting and junk like that
            var lines = new List<string>
            {
                "---",
                "Title:",
                $"  {pullRequest.Descriptor.Title.Trim()}",
                "",
                "Description:",
                $"  {pullRequest.Descriptor.Description.Trim()}",
                "",
                "Url:",
                $"  {pullRequest.Url}",
                "",
                "Checks:"
            };

            var titleWidth = pullRequest.Checks.Max(x => x.Name.Length);

            //Checks here
            foreach (var check in pullRequest.Checks)
            {
                var symbol = check switch
                {
                    { Status: CheckStatus.InProgress } => "\uD83D\uDD35", //Large Blue Circle
                    { Status: CheckStatus.Completed, Result: CheckResult.Success } => "\uD83D\uDFE2", //Large Green Circle
                    { Status: CheckStatus.Completed, Result: CheckResult.Failure } => "\uD83D\uDD34", //Large Red Circle
                    { Status: CheckStatus.Completed, Result: CheckResult.Timeout } => "\uD83D\uDC22", //Turtle
                    { Status: CheckStatus.Completed, Result: CheckResult.Skipped } => "\uD83E\uDEA2", //Knot (Kinda like a skipping rope? Really reaching)
                    { Status: CheckStatus.Completed, Result: CheckResult.Neutral } => "\uD83E\uDD37", //Shrug
                    _ => "\u25EF\uFE0F"
                };

                lines.Add($"{symbol,4} {check.Name.PadRight(titleWidth)} -- {check.Url}");
            }

            return lines;
        }
    }
}
