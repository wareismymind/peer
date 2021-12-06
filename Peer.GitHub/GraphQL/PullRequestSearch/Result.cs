namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class Result : NodeList<PullRequest>
    {
        public int IssueCount { get; set; }
        //public List<PullRequest> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
#nullable enable
}
