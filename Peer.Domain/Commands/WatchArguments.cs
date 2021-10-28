using System;

namespace Peer.Domain.Commands
{
    public class WatchArguments
    {
        public TimeSpan IntervalSeconds { get; } = TimeSpan.FromSeconds(30);

        public WatchArguments(TimeSpan intervalSeconds)
        {
            IntervalSeconds = intervalSeconds;
        }
    }
}
