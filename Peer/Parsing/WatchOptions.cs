using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
