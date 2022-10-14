using System;
using Peer.Apps;

namespace Peer.Verbs;


// public class SubVerbTypeBinding<TSuper, TSub> : ISubVerbTypeBinding<TSuper>
// {
//     public Type Super => typeof(TSuper);
//     public Type Sub => typeof(TSub);
// }

public interface ISubVerbTypeBinding<TSuper>
{
    public Type Super { get; }
    public Type Sub { get; }
}

public interface ISubVerb<TParent> : IVerb
{
}
