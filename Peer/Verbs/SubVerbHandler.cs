using System;
using Peer.Apps;

namespace Peer.Verbs;

public interface ISubVerbTypeBinding<TSuper>
{
    public Type Super { get; }
    public Type Sub { get; }
}

public interface ISubVerb<TParent> : IVerb
{
}
