namespace Peer.Domain.Models
{
    public class PeerPullRequest
    {
        public PeerPullRequest(string title, object assignee)
        {
            Title = title;
            Assignee = assignee;
        }

        public string Title { get; set; }
        public object Assignee { get; set; }
    }
}
