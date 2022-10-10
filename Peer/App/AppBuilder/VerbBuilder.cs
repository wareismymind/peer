using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Peer.Parsing.CommandLine;
using Peer.Verbs;

namespace Peer.App.AppBuilder;

public class VerbBuilder<T>
{
    private readonly IServiceCollection _services;
    private readonly List<IVerb> _subs = new();
    public VerbBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public VerbBuilder<T> WithHandler<THandler>() where THandler : class, IHandler<T>
    {
        _services.AddSingleton<IHandler<T>, THandler>();
        _services.AddSingleton<IHandler, HandlerWrapper<T>>();
        return this;
    }

    public VerbBuilder<T> WithActionHandler(Func<T, IServiceCollection, CancellationToken, Task<int>> handler)
    {
        _services.AddSingleton<IHandler<T>>(new ActionHandler<T>(handler));
        _services.AddSingleton<IHandler, HandlerWrapper<T>>();
        return this;
    }

    public VerbBuilder<T> WithSubVerb<TSub>(Action<VerbBuilder<TSub>>? config = null)
    {
        _services.AddSingleton<ISubVerb<T>, SubVerb<T, TSub>>();
        var sub = new SubVerbBuilder<T, TSub>(_services);
        config?.Invoke(sub);
        return this;
    }

    public VerbBuilder<T> WithCustomHelp<THelp>() where THelp : class, IHelpTextFormatter<T>
    {
        _services.AddSingleton<IHelpTextFormatter<T>, THelp>();
        return this;
    }
}
