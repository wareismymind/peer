using System;
using System.Collections.Generic;
using System.Linq;
using wimm.Secundatives.Extensions;


namespace Peer.Domain.Commands
{
    //TODO:CN -- Ansi term code support
    public class DetailsFormatter : IDetailsFormatter
    {
        private const string _pad = "  ";

        private readonly ICheckSymbolProvider _symbolProvider;

        public DetailsFormatter(ICheckSymbolProvider symbolProvider)
        {
            _symbolProvider = symbolProvider;
        }

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

            if (!pullRequest.Checks.Any())
            {
                return lines;
            }

            lines.Add("Checks:");
            var titleWidth = pullRequest.Checks.Max(x => x.Name.Length);

            //Checks here
            foreach (var check in pullRequest.Checks)
            {
                var symbol = _symbolProvider.GetSymbol(check.Status, check.Result)
                    .UnwrapOr("\u25EF\uFE0F"); //Large white circle
                    
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
