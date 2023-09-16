using Microsoft.Extensions.DependencyInjection;
using Peer.Configuration;
using wimm.Secundatives;

namespace Peer.Apps;

public interface IServiceSetupHandler
{
    Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services);
}
