using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peer.GitHub;
using Peer.GitHub.GraphQL.PullRequestSearch;
using Peer.Verbs;

namespace Peer.Apps.AppBuilder;

public class VerbBuilder<T>(IServiceCollection services)
{
    private readonly IServiceCollection _services = services;

    public VerbBuilder<T> WithHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]THandler>() where THandler : class, IHandler<T>
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

    public VerbBuilder<T> WithCustomHelp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]  THelp>() where THelp : class, IHelpTextFormatter<T>
    {
        _services.AddSingleton<IHelpTextFormatter<T>, THelp>();
        return this;
    }

    public VerbBuilder<T> WithRunTimeConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRegHandler>()
        where TRegHandler : class, IRunTimeConfigHandler
    {
        _services.TryAddSingleton<TRegHandler>();
        _services.AddSingleton<IRunTimeConfigHandler<T>, RunTimeConfigMapping<T, TRegHandler>>();
        return this;
    }
}
