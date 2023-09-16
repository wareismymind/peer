using System;

namespace Peer.Configuration.CommandConfigs
{
    public class ShowConfig
    {
        private const int _defaultTimeoutSeconds = 10;
        private const int _defaultWatchIntervalSeconds = 30;
        private const int _defaultWatchMaxConsecutiveShowFailures = 5;

        public TimeSpan TimeoutSeconds { get; set; }
        public TimeSpan WatchIntervalSeconds { get; set; }

        public int WatchMaxConsecutiveShowFailures { get; set; }

        public ShowConfig(
            int? timeoutSeconds,
            int? watchIntervalSeconds,
            int? watchMaxConsecutiveShowFailures)
        {
            TimeoutSeconds = TimeSpan.FromSeconds(timeoutSeconds ?? _defaultTimeoutSeconds);
            WatchIntervalSeconds = TimeSpan.FromSeconds(watchIntervalSeconds ?? _defaultWatchIntervalSeconds);
            WatchMaxConsecutiveShowFailures =
                watchMaxConsecutiveShowFailures ?? _defaultWatchMaxConsecutiveShowFailures;
        }
    }
}
