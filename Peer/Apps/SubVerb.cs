using System.Collections.Generic;
using Peer.Apps.AppBuilder;
using Peer.Verbs;

namespace Peer.Apps;

public class SubVerb<TVerb, TSubVerb> : Verb<TSubVerb>, ISubVerb<TVerb>
{
    public SubVerb(
        IEnumerable<ISubVerb<TSubVerb>> subs,
        IHandler<TSubVerb> handler,
        IRunTimeConfigHandler<TSubVerb>? runTimeHandler = null,
        IHelpTextFormatter<TSubVerb>? helpFormatter = null)
        : base(
            subs,
            runTimeHandler,
            handler,
            helpFormatter)
    {
    }
}
