using System;

namespace Peer.Domain.Commands
{
    public class OpenArguments
    {
        public string Partial { get; }

        public OpenArguments(string partial)
        {
            Partial = partial ?? throw new ArgumentNullException(nameof(partial));
        }
    }
}
