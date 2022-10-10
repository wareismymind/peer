using System;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.App.AppBuilder;

public class AppBuilder
{
    private readonly IServiceCollection _services;

    public AppBuilder(IServiceCollection services)
    {
        _services = services;
        _services.AddSingleton<CommandLineParser>();
        _services.AddSingleton<App>();
    }

    public AppBuilder WithParseTimeServiceConfig(
        Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        config.Invoke(_services);
        return this;
    }

    public AppBuilder WithSharedServiceConfig(Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        _services.AddSingleton<IServiceSetupHandler>(new FuncServiceSetupHandler(config));
        return this;
    }

    public VerbBuilder<TVerb> WithVerb<TVerb>()
    {
        if (typeof(TVerb).GetCustomAttribute(typeof(VerbAttribute)) == null)
        {
            throw new ArgumentException($"The type must have the {nameof(VerbAttribute)} Attribute");
        }
        _services.AddSingleton<IVerb, Verb<TVerb>>();
        return new VerbBuilder<TVerb>(_services);
    }

    public App Build()
    {
        var sp = _services.BuildServiceProvider();
        return sp.GetRequiredService<App>();
    }
}
