namespace Peer.Domain.Models
{
    public class PeerPullRequest
    {
        public string Title { get; set; }
        public string Assignee { get; set; }
        public long Id { get; set; }

        public PeerPullRequest(string title, string assignee, long id)
        {
            Title = title;
            Assignee = assignee;
            Id = id;
        }
    }
}
