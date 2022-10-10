using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Peer.Verbs;

namespace Peer.Parsing.CommandLine;

public class Verb<TVerb> : IVerb
{
    public IEnumerable<IVerb> Subs { get; }
    public IHelpTextFormatter? CustomHelp { get; }

    public IHandler? Handler { get; }

    public Type Type => typeof(TVerb);
    public IEnumerable<Type> SubVerbs => Subs.Select(x => x.Type);

    public Verb(
        IEnumerable<ISubVerb<TVerb>> subs,
        IHandler<TVerb>? handler = null, //CN - should probably have just a different name for top level verbs
        IHelpTextFormatter<TVerb>? helpFormatter = null)
    {
        CustomHelp = helpFormatter;
        Handler = handler != null ? new HandlerWrapper<TVerb>(handler) : null;
        Subs = subs.ToList();
    }


    public string Name => Type.GetCustomAttributes(typeof(VerbAttribute), false)
        .OfType<VerbAttribute>()
        .Select(x => x.Name)
        .First();
}

