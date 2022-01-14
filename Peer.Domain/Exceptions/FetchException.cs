using System;
using System.Runtime.Serialization;

namespace Peer.Domain.Exceptions
{
    public class FetchException : Exception
    {
        public FetchException() { }

        public FetchException(string? message) : base(message) { }

        public FetchException(string? message, Exception? innerException) : base(message, innerException) { }

        protected FetchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
