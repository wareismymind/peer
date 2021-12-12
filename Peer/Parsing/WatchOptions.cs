using System;
using Peer.Domain.Commands;

namespace Peer.Parsing
{
    public class WatchOptions
    {
        public int WatchIntervalSeconds { get; set; } = 30;

        public WatchArguments Into()
        {
            return new WatchArguments(TimeSpan.FromSeconds(WatchIntervalSeconds));
        }
    }
}
