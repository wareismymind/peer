using System;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Apps.AppBuilder;

public class AppBuilder
{
    private readonly IServiceCollection _services;

    public AppBuilder(IServiceCollection services)
    {
        _services = services;
        _services.AddSingleton<ICommandLineParser,CommandLineParser>();
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

    public AppBuilder WithVerb<TVerb>(Action<VerbBuilder<TVerb>> config)
    {
        if (typeof(TVerb).GetCustomAttribute(typeof(VerbAttribute)) == null)
        {
            throw new ArgumentException($"The type must have the {nameof(VerbAttribute)} Attribute");
        }

        var builder = new VerbBuilder<TVerb>(_services);
        config.Invoke(builder);
        _services.AddSingleton<IVerb, Verb<TVerb>>();
        return this;
    }

    public App Build()
    {
        var sp = _services.BuildServiceProvider();
        return sp.GetRequiredService<App>();
    }
}
