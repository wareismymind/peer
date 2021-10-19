using System;

namespace Peer.Domain.Commands
{
    public class Open
    {
        public string Partial { get; }

        public Open(string partial)
        {
            Partial = partial ?? throw new ArgumentNullException(nameof(partial));
        }
    }
}
