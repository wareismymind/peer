using System;
using System.Collections.Generic;
using System.Linq;

namespace Peer.Domain.Commands
{
    //TODO:CN -- Ansi term code support
    public class DetailsFormatter : IDetailsFormatter
    {
        private const string _pad = "  ";

        public IList<string> Format(PullRequest pullRequest)
        {
            var lines = new List<string>
            {
                "---",
                "Title:",
                $"{_pad}{pullRequest.Descriptor.Title.Trim()}",
                string.Empty,
            };
            lines.AddRange(SplitAndPad(pullRequest.Descriptor.Description));
            lines.Add(string.Empty);
            lines.Add("Url:");
            lines.Add($"{_pad}{pullRequest.Url}");
            lines.Add(string.Empty);
            lines.Add("Checks:");


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
                    _ => "\u25EF\uFE0F" //Large white circule
                };

                lines.Add($"{_pad}{symbol,4} {check.Name.PadRight(titleWidth)} -- {check.Url}");
            }

            return lines;
        }

        private static string[] SplitAndPad(string input)
        {
            var split = input.Trim().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return split.Select(x => $"{_pad}{x}").ToArray();
        }
    }
}
