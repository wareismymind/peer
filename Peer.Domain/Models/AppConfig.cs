namespace Peer.Domain.Models
{
    public class AppConfig
    {
        public GitlabConfig? Gitlab { get; set; }
        public GithubConfig? Github { get; set; }
        public AzdoConfig? Azdo { get; set; }
    }

    public class GithubConfig
    {
        public string? ProductHeaderValue { get; set; }
        public string? Token { get; set; }
        public string? Name { get; set; }
        public string? Org { get; set; }
    }

    public class GitlabConfig
    {
        public string? CollectionUri { get; set; }
        public string? Token { get; set; }
        public string? Name { get; set; }
        public string? Org { get; set; }
    }

    public class AzdoConfig
    {
        public string? CollectionUri { get; set; }
        public string? Org { get; set; }
        public string? Token { get; set; }
    }
}
