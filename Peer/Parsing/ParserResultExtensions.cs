using CommandLine;

namespace Peer.Parsing
{
    public static class ParserResultExtensions
    {
        public static bool Is<TValue>(this ParserResult<object> parserResult)
        {
            return parserResult.TypeInfo.Current == typeof(TValue);
        }
    }
}
