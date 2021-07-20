using Octokit;

namespace Peer.Domain.Models
{
    public class UserClient
    {
        public GitHubClient Client { get; set; }
    }
}
