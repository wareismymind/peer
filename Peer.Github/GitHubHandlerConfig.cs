using wimm.Secundatives;

namespace Peer.GitHub
{
    public class GithubHandlerConfig
    {
        public string Name { get; set; } = string.Empty;
        public GithubPeerConfigDto? Configuration { get; set; }

        public Result<GithubPeerConfig, GithubConfigError> Into()
        {
            if (Configuration == null)
            {
                return GithubConfigError.ConfigurationBlockMissing;
            }

            if (string.IsNullOrWhiteSpace(Configuration.AccessToken))
            {
                return GithubConfigError.AccessTokenInvalid;
            }

            if (Configuration.Username != null && Configuration.Username.All(x => char.IsWhiteSpace(x)))
            {
                return GithubConfigError.UsernameInvalid;
            }

            var realizedExcluded = Configuration.ExcludedOrgs.ToList();
            var realizedIncluded = Configuration.ExcludedOrgs.ToList();

            if (realizedExcluded.Any() && realizedIncluded.Any())
            {
                return GithubConfigError.InvalidOrgConfig;
            }

            return new GithubPeerConfig(
                Name,
                Configuration.AccessToken,
                Configuration.Username,
                realizedIncluded,
                realizedExcluded);
        }
    }
}
