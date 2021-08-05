namespace Peer.Domain.Models
{
    public class PeerPullRequest
    {
        public PeerPullRequest(string title, object assignee, long id)
        {
            Title = title;
            Assignee = assignee;
            Id = id;
        }

        public string Title { get; set; }
        public object Assignee { get; set; }
        public long Id { get; set; }
    }
}
