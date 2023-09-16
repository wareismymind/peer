using System.Diagnostics.CodeAnalysis;
using Peer.ConfigSections;

namespace Peer.Parsing
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public class PeerOptions
    {
        public int? ShowTimeoutSeconds { get; set; }
        public int? WatchIntervalSeconds { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public class PeerEnvironmentOptions
    {
        public string ConfigPath { get; set; } = Constants.DefaultConfigPath;
        public string? Editor { get; set; }
    }
}
