using System.Collections.Generic;

namespace Peer.GitHub
{
    public class GitHubPeerConfigDto
    {
        public string? AccessToken { get; set; } //TODO:CN -- Maybe make this Variant<AccessToken,SecretRef?>
        public string? Username { get; set; }
        public IList<string> Orgs { get; set; } = new List<string>();
        public IList<string> ExcludedOrgs { get; set; } = new List<string>();
        public int Count { get; set; } = 20;
    }
}
