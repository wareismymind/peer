using CommandLine;

namespace Peer.Verbs
{
    [Verb("open", isDefault: false, HelpText = "Use the default browser to open the pull request URL")]
    public class OpenOptions
    {
        [Value(0, Required = true, HelpText = HelpConstants.PartialHelp)]
        public string? Partial { get; set; }
    }
}
