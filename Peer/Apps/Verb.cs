using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommandLine;
using Peer.Apps.AppBuilder;
using Peer.Verbs;

namespace Peer.Apps;

public class Verb<TVerb> : IVerb
{
    public IEnumerable<IVerb> Subs { get; }
    public IHelpTextFormatter? CustomHelp { get; }

    public IHandler? Handler { get; }
    public IRunTimeConfigHandler? RunTimeConfigHandler { get; }


    public Type Type => typeof(TVerb);
    public IEnumerable<Type> SubVerbs => Subs.Select(x => x.Type);



    public Verb(
        IEnumerable<ISubVerb<TVerb>> subs,
        IRunTimeConfigHandler<TVerb>? runtimeHandler = null,
        IHandler<TVerb>? handler = null, //CN - should probably have just a different name for top level verbs
        IHelpTextFormatter<TVerb>? helpFormatter = null)
    {
        RunTimeConfigHandler = runtimeHandler;
        CustomHelp = helpFormatter;
        Handler = handler != null ? new HandlerWrapper<TVerb>(handler) : null;
        var subVerbs = subs.ToList();
        Subs = subVerbs;
        if (subVerbs.Count == 0 && handler == null)
        {
            throw new ArgumentException("Verbs must have a handler or sub-verbs");
        }
    }


    public string Name => Type.GetCustomAttributes(typeof(VerbAttribute), false)
        .OfType<VerbAttribute>()
        .Select(x => x.Name)
        .First();

}

