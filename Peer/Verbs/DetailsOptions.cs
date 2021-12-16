using CommandLine;

namespace Peer.Verbs
{
    [Verb("details", isDefault: false, HelpText = "Show additional details about a PullRequest")]
    public class DetailsOptions
    {
        [Value(0, Required = true, HelpText = HelpConstants.PartialHelp)]
        public string? Partial { get; set; }
    }
}
