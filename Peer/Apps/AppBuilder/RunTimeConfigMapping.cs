using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Apps.AppBuilder;

public class RunTimeConfigMapping<TVerb, THandler> : IRunTimeConfigHandler<TVerb> where THandler : IRunTimeConfigHandler
{
    private readonly IRunTimeConfigHandler _handler;

    public RunTimeConfigMapping(THandler configHandler)
    {
        _handler = configHandler;
    }

    public Result<None, ConfigError> ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        return _handler.ConfigureServices(services, config);
    }
}
