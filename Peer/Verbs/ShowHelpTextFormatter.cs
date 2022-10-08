using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Peer.Domain;
using Peer.Parsing;

namespace Peer.Verbs
{
    public class ShowHelpTextFormatter : IHelpTextFormatter<ShowOptions>
    {
        private readonly ISymbolProvider _symbolProvider;

        public ShowHelpTextFormatter(ISymbolProvider symbolProvider)
        {
            _symbolProvider = symbolProvider;
        }

        public HelpText GetHelpText(ParserResult<object> parserResult)
        {
            var help = HelpText.AutoBuild(parserResult);
            help.AddPreOptionsLine("Shows a list of your pull requests and their statuses with applied transforms");

            help.AddPostOptionsLines(new[]
            {
                "-- SYMBOLS --",
                "",
                "The symbols in the show command and their meanings are shown below:",
                ""
            });

            var statuses = Enum.GetValues<PullRequestStatus>();
            var maxWidth = statuses.Max(x => x.ToString().Length);

            help.AddPostOptionsLines(statuses.Select(x => $"  {x.ToString().PadRight(maxWidth)} => {_symbolProvider.GetSymbol(x)}"));
            help.AddPostOptionsLine("");

            help.AddPostOptionsLines(new[]
            {
                "-- SORTING --",
                "",
                "The sort option can use a number of different keys and default to ascending order. The avaliable keys are:",
                ""
            });

            help.AddPostOptionsLines(SelectorMap.Keys.Select(x => $"  {x}"));
            help.AddPostOptionsLine("");
            help.AddPostOptionsLines(new[]
            {
                "-- FILTERING --",
                "",
                "Filtering can be done on any of the sort keys. The filter syntax is: 'key:value', negation is provided by prepending a '!' before the filter key.",
                "",
                "  - Numbers can be matched only for equality (or inequality)",
                "  - Strings will be matched by regex",
                "  - The pull request status filter is an enum based filter and supports all of the statuses from the previous section."
            });

            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("The filter key types are listed below:");
            help.AddPostOptionsLine("");
            maxWidth = SelectorMap.Keys.Max(x => x.Length);
            help.AddPostOptionsLines(SelectorMap.Keys.Select(x => $"  {x.PadRight(maxWidth)} => {SelectorMap.TryGetSelector(x).Value.ReturnType.Name}"));
            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("");

            return help;
        }
    }
}
