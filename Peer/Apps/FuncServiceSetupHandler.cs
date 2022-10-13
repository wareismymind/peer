using System;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Apps;

public class FuncServiceSetupHandler : IServiceSetupHandler
{
    private readonly Func<IServiceCollection, Result<IServiceCollection, ConfigError>> _config;

    public FuncServiceSetupHandler(Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        _config = config;
    }

    public Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services)
    {
        return _config(services);
    }
}
