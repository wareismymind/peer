namespace Peer.Domain.Models
{
    public class ConfigModel
    {
        public GithubConfig Github { get; set; } = new();
        public AzdoConfig Azdo { get; set; } = new();
    }

    public class GithubConfig
    {
        public string ProductHeaderValue { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string Org { get; set; }
    }

    public class AzdoConfig
    {
        public string CollectionUri { get; set; }
        public string Org { get; set; }
        public string Token { get; set; }
    }
}
