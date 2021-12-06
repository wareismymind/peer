﻿namespace Peer.Domain.Commands
{
    public class OpenArguments
    {
        public PartialIdentifier Partial { get; }

        public OpenArguments(PartialIdentifier partial)
        {
            Partial = partial;
        }
    }
}
