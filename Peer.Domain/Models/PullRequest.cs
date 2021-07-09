using System;

namespace Peer.Domain.Models
{
    public class PullRequest
    {
        public string Title { get; set; }
        public string Actor { get; set; }
        public DateTime Date { get; set; }
    }
}
