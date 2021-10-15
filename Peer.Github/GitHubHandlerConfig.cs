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

            if (string.IsNullOrWhiteSpace(Configuration.Username))
            {
                return GithubConfigError.UsernameMissing;
            }

            return new GithubPeerConfig(
                Name,
                Configuration.AccessToken,
                Configuration.Username,
                Configuration.Orgs,
                Configuration.ExcludedOrgs);
        }
    }
}
