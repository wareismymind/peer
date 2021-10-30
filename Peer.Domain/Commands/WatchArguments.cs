using System;

namespace Peer.Domain.Commands
{
    public class WatchArguments
    {
        public TimeSpan IntervalSeconds { get; }

        public WatchArguments(TimeSpan intervalSeconds)
        {
            IntervalSeconds = intervalSeconds;
        }
    }
}
