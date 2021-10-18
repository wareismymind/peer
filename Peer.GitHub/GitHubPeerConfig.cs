using System;
using System.Collections.Generic;

namespace Peer.GitHub
{
    public class GitHubPeerConfig
    {
        public string Name { get; }
        public string AccessToken { get; } //TODO:CN -- Maybe make this Variant<AccessToken,SecretRef?>
        public string? Username { get; }
        public IEnumerable<string> Orgs { get; }
        public IEnumerable<string> ExcludedOrgs { get; }

        public GitHubPeerConfig(string name, string accessToken, string? username, IEnumerable<string> orgs, IEnumerable<string> excludedOrgs)
        {
            Name = name ?? throw new ArgumentNullException(name);
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            Username = username;
            Orgs = orgs ?? throw new ArgumentNullException(nameof(orgs));
            ExcludedOrgs = excludedOrgs ?? throw new ArgumentNullException(nameof(excludedOrgs));
        }
    }
}
