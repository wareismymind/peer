using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Apps.AppBuilder;

// Shaaapes https://www.youtube.com/watch?v=evthRoKoE1o
public interface IRunTimeConfigHandler
{
    Result<None, ConfigError> ConfigureServices(IServiceCollection services, IConfiguration config);
}

public interface IRunTimeConfigHandler<TVerb> : IRunTimeConfigHandler
{
}
