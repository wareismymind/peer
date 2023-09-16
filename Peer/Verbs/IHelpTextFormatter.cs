using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;

namespace Peer.Verbs
{
    public interface IHelpTextFormatter<TOptions> : IHelpTextFormatter
    {
    }

    public interface IHelpTextFormatter
    {
        HelpText GetHelpText(ParserResult<object> parserResult);
    }
}
