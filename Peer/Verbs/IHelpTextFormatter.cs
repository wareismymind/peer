using CommandLine;
using CommandLine.Text;

namespace Peer.Verbs
{
    public interface IHelpTextFormatter<TOptions>
    {
        HelpText GetHelpText(ParserResult<object> parserResult);
    }
}
