using System.Linq;
using wimm.Secundatives;

namespace Peer.GitHub
{
    public class GitHubHandlerConfig
    {
        public string Name { get; set; } = string.Empty;
        public GitHubPeerConfigDto? Configuration { get; set; }

        public Result<GitHubPeerConfig, GitHubConfigError> Into()
        {
            if (Configuration == null)
            {
                return GitHubConfigError.ConfigurationBlockMissing;
            }

            if (string.IsNullOrWhiteSpace(Configuration.AccessToken))
            {
                return GitHubConfigError.AccessTokenInvalid;
            }

            if (Configuration.Username != null && Configuration.Username.All(x => char.IsWhiteSpace(x)))
            {
                return GitHubConfigError.UsernameInvalid;
            }

            var realizedExcluded = Configuration.ExcludedOrgs.ToList();
            var realizedIncluded = Configuration.Orgs.ToList();

            if (realizedExcluded.Any() && realizedIncluded.Any())
            {
                return GitHubConfigError.InvalidOrgConfig;
            }

            if (Configuration.PageSize.HasValue && (Configuration.PageSize < 1 || Configuration.PageSize > 100))
            {
                return GitHubConfigError.PageSizeInvalid;
            }

            return new GitHubPeerConfig(
                Name,
                Configuration.AccessToken,
                Configuration.Username,
                realizedIncluded,
                realizedExcluded,
                Configuration.PageSize
                );
        }
    }
}
