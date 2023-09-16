using Peer.Configuration.CommandConfigs;

namespace Peer.ConfigSections
{
    public class ShowConfigSection
    {
        public int? TimeoutSeconds { get; set; }

        public int? WatchIntervalSeconds { get; set; }

        public int? WatchMaxConsecutiveShowFailures { get; set; }

        public ShowConfig Into() => new(
            TimeoutSeconds,
            WatchIntervalSeconds,
            WatchMaxConsecutiveShowFailures);
    }
}
