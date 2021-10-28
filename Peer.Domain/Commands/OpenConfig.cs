using System;

namespace Peer.Domain.Commands
{
    public class OpenConfig
    {
        public string Partial { get; }

        public OpenConfig(string partial)
        {
            Partial = partial ?? throw new ArgumentNullException(nameof(partial));
        }
    }
}
