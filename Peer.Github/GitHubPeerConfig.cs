namespace Peer.GitHub
{
    public class GithubPeerConfig
    {
        public string Name { get; }
        public string AccessToken { get; } //TODO:CN -- Maybe make this Variant<AccessToken,SecretRef?>
        public string Username { get; }
        public IEnumerable<string> Orgs { get; }
        public IEnumerable<string> ExcludedOrgs { get; }

        public GithubPeerConfig(string name, string accessToken, string username, IEnumerable<string> orgs, IEnumerable<string> excludedOrgs)
        {
            Name = name ?? throw new ArgumentNullException(name);
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Orgs = orgs ?? throw new ArgumentNullException(nameof(orgs));
            ExcludedOrgs = excludedOrgs ?? throw new ArgumentNullException(nameof(excludedOrgs));
        }
    }
}
