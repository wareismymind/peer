using System;
using System.Runtime.Serialization;

namespace Peer.Domain.Exceptions
{
    public class UnreachableException : Exception
    {
        public UnreachableException()
            : base("This branch was expected to be unreachable")
        {
        }

        protected UnreachableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
