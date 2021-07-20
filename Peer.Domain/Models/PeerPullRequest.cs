namespace Peer.Domain.Models
{
    public class PeerPullRequest
    {
        private string _title;
        private object _assignee;

        public PeerPullRequest(string Title, object Assignee)
        {
            _title = Title;
            _assignee = Assignee;
        }

        public string Title { get; set; }
        public string Assignee { get; set; }
    }
}
