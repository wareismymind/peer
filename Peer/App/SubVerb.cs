using System.Collections.Generic;
using Peer.Verbs;

namespace Peer.App;

public class SubVerb<TVerb, TSubVerb> : Verb<TSubVerb>, ISubVerb<TVerb>
{
    public SubVerb(
        IEnumerable<ISubVerb<TSubVerb>> subs,
        IHandler<TSubVerb> handler,
        IHelpTextFormatter<TSubVerb>? helpFormatter = null)
        : base(
            subs,
            handler,
            helpFormatter)
    {
    }
}
