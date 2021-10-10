using CommandLine;

namespace Peer.Verbs
{
    [Verb("open", isDefault: false, HelpText = "Use the default browser to open the pull request URL")]
    public class OpenOptions
    {
        [Option(Required = true)]
        public string? Identifier { get; set; }
    }
}
