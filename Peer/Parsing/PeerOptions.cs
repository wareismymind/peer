using Peer.ConfigSections;

namespace Peer.Parsing
{
    public class PeerOptions
    {
        public int? ShowTimeoutSeconds { get; set; }
        public int? WatchIntervalSeconds { get; set; }
    }

    public class PeerEnvironmentOptions
    {
        public string ConfigPath { get; set; } = Constants.DefaultConfigPath;
        public string? Editor { get; set; }
    }
}
