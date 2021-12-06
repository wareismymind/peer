namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class Commit
    {
        public StatusCheckRollup StatusCheckRollup { get; set; }
        public NodeList<CheckSuite> CheckSuites { get; set; }
    }

    public class CheckSuite
    {
        public NodeList<CheckRun> CheckRuns { get; set; }
    }
#nullable enable
}
