using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Peer.Domain;

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
            help.AddPostOptionsLine("The symbols in the show command and their meanings are shown below:");
            help.AddPostOptionsLine("");

            var statuses = Enum.GetValues<PullRequestStatus>();
            var maxWidth = statuses.Max(x => x.ToString().Length);

            help.AddPostOptionsLines(statuses.Select(x => $"  {x.ToString().PadRight(maxWidth)} => {_symbolProvider.GetSymbol(x)}"));
            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("");
            return help;
        }
    }
}
