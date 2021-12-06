using CommandLine;

namespace Peer.Verbs
{
    [Verb("details", isDefault: false, HelpText = "Show additional details about a PullRequest")]
    public class DetailsOptions
    {
        [Value(0, Required = true,
        HelpText = "A full or partial pull request identifier of the form provider/org/repo/id" +
        " partials can be built up from combinations of sections. For example: '6' to get" +
        " pull the pull request with id 6 provided there is only one. If there are more than one match" +
        " you'll have to disambiguate by adding additional qualifiers eg. repoName/6")]
        public string? Partial { get; set; }
    }
}
