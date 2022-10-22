using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peer.Domain.Configuration;
using Peer.GitHub;
using Peer.GitHub.GraphQL.PullRequestSearch;
using Peer.Verbs;
using wimm.Secundatives;

namespace Peer.Apps.AppBuilder;

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

    public VerbBuilder<T> WithRunTimeConfig<TRegHandler>() where TRegHandler : class, IRunTimeConfigHandler
    {
        _services.TryAddSingleton<TRegHandler>();
        _services.AddSingleton<IRunTimeConfigHandler<T>, RunTimeConfigMapping<T, TRegHandler>>();
        return this;
    }
}

public class ProviderLoader : IRunTimeConfigHandler
{
    private readonly List<IRegistrationHandler> _registrationHandlers;

    public ProviderLoader(IEnumerable<IRegistrationHandler> registrationHandlers)
    {
        _registrationHandlers = registrationHandlers.ToList();
    }

    public Result<None, ConfigError> ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        var configLoader = new ConfigurationService(_registrationHandlers, config);
        return configLoader.RegisterProvidersForConfiguration(services);
    }
}
