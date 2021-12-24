using CommandLine;

namespace Peer.Verbs
{
    [Verb("show", isDefault: true, HelpText = "Display pull requests assigned to your account")]
    public class ShowOptions
    {
        [Option(shortName: 's', longName: "sort", Required = false)]
        public string? Sort { get; set; }

        [Option(shortName: 'w', longName: "watch", Required = false)]
        public bool Watch { get; set; }

        [Option(shortName: 'c', longName: "count", Required = false, Default = 40)]
        public int Count { get; set; }
    }
}
