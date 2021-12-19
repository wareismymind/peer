using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Peer.Domain;

namespace Peer.Verbs
{
    public class DetailsHelpTextFormatter : IHelpTextFormatter<DetailsOptions>
    {
        private readonly ICheckSymbolProvider _symbolProvider;

        public DetailsHelpTextFormatter(ICheckSymbolProvider symbolProvider)
        {
            _symbolProvider = symbolProvider;
        }

        public HelpText GetHelpText(ParserResult<object> parserResult)
        {
            var help = HelpText.AutoBuild(parserResult);
            help.AddPreOptionsLine("Shows the details of a specific pull request including description and check info");

            help.AddPostOptionsLine("Since Checks have both a completion state and a result the mapping between states and symbols in the details command are shown below:");
            help.AddPostOptionsLine("");

            var existingPairs = new List<(CheckStatus status, CheckResult result, string symbol)>();
            foreach (var status in Enum.GetValues<CheckStatus>())
            {
                foreach (var result in Enum.GetValues<CheckResult>())
                {
                    var symbol = _symbolProvider.GetSymbol(status, result);

                    if (symbol.Exists)
                    {
                        existingPairs.Add((status, result, symbol.Value));
                    }
                }
            }

            var statusWidth = existingPairs.Max(x => x.status.ToString().Length);
            var resultWidth = existingPairs.Max(x => x.result.ToString().Length);

            help.AddPostOptionsLines(existingPairs.Select(
                triple => $"Status: {triple.status.ToString().PadRight(statusWidth)} " +
                $"Result: { triple.result.ToString().PadRight(resultWidth)} => {triple.symbol}"));

            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("");

            return help;
        }
    }
}
