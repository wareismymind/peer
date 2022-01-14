namespace Peer.Domain.Configuration.CommandConfigs
{
    public class ShowConfig
    {
        private const int _defaultRetryCutoffSeconds = 10;
        private const int _defaultMaxRequestRetries = 5;
        private const int _defaultWatchIntervalSeconds = 30;
        private const int _defaultWatchMaxConsecutiveShowFailures = 5;

        public int RetryCutoffSeconds { get; private set; }

        public int MaxRequestRetries { get; private set; }

        public int WatchIntervalSeconds { get; private set; }

        public int WatchMaxConsecutiveShowFailures { get; private set; }

        public ShowConfig(
            int? retryCutoffSeconds,
            int? maxRequestRetries,
            int? watchIntervalSeconds,
            int? watchMaxConsecutiveShowFailures)
        {
            RetryCutoffSeconds = retryCutoffSeconds ?? _defaultRetryCutoffSeconds;
            MaxRequestRetries = maxRequestRetries ?? _defaultMaxRequestRetries;
            WatchIntervalSeconds = watchIntervalSeconds ?? _defaultWatchIntervalSeconds;
            WatchMaxConsecutiveShowFailures =
                watchMaxConsecutiveShowFailures ?? _defaultWatchMaxConsecutiveShowFailures;
        }
    }
}
