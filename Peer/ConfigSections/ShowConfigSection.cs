using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain.Configuration.CommandConfigs;

namespace Peer.ConfigSections
{
    public class ShowConfigSection
    {
        public int RetryCutoffSeconds { get; set; }

        public int MaxRequestRetries { get; set; }

        public int WatchIntervalSeconds { get; set; }

        public int WatchMaxConsecutiveShowFailures { get; set; }

        public ShowConfig Into() => new(
            RetryCutoffSeconds,
            MaxRequestRetries,
            WatchIntervalSeconds,
            WatchMaxConsecutiveShowFailures);
    }
}
