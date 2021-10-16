namespace Peer.GitHub
{
    public class GithubPeerConfigDto
    {
        public string? AccessToken { get; set; } //TODO:CN -- Maybe make this Variant<AccessToken,SecretRef?>
        public string? Username { get; set; }
        public IEnumerable<string> Orgs { get; set; } = new List<string>();
        public IEnumerable<string> ExcludedOrgs { get; set; } = new List<string>();
    }
}
