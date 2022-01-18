namespace Peer.Domain.Configuration.CommandConfigs
{
    public class ShowConfig
    {
        private const int _defaultTimeoutSeconds = 10;
        private const int _defaultWatchIntervalSeconds = 30;
        private const int _defaultWatchMaxConsecutiveShowFailures = 5;

        public int TimeoutSeconds { get; private set; }


        public int WatchIntervalSeconds { get; private set; }

        public int WatchMaxConsecutiveShowFailures { get; private set; }

        public ShowConfig(
            int? timeoutSeconds,
            int? watchIntervalSeconds,
            int? watchMaxConsecutiveShowFailures)
        {
            TimeoutSeconds = timeoutSeconds ?? _defaultTimeoutSeconds;
            WatchIntervalSeconds = watchIntervalSeconds ?? _defaultWatchIntervalSeconds;
            WatchMaxConsecutiveShowFailures =
                watchMaxConsecutiveShowFailures ?? _defaultWatchMaxConsecutiveShowFailures;
        }
    }
}
