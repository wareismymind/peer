using System;

namespace Peer.Domain.Commands
{
    public class OpenArguments
    {
        public PartialIdentifier Partial { get; }

        public OpenArguments(PartialIdentifier partial)
        {
            Partial = partial ?? throw new ArgumentNullException(nameof(partial));
        }
    }
}
