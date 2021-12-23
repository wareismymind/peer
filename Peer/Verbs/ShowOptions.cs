using CommandLine;

namespace Peer.Verbs
{
    [Verb("show", isDefault: true, HelpText = "Display pull requests assigned to your account")]
    public class ShowOptions
    {
        [Option(shortName: 's', longName: "sort", Required = false, HelpText = "The sort key and direction you would like to use. " +
            "The default sort direction is ascending. <key>:[(asc|ascending)|(desc|descending)]")]
        public string? Sort { get; set; }

        [Option(shortName: 'w', longName: "watch", Required = false)]
        public bool Watch { get; set; }
    }
}
