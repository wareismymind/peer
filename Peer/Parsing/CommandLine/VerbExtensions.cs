using System.Linq;
using CommandLine;

namespace Peer.Parsing.CommandLine;

public static class VerbExtensions
{
    public static string GetOptionLongName<T>(this T value, string propName) where T : notnull
    {
        return value.GetType()
            .GetProperties()
            .First(x => x.Name == propName)
            .GetCustomAttributes(typeof(OptionAttribute), false)
            .OfType<OptionAttribute>()
            .First()
            .LongName;
    }
}
