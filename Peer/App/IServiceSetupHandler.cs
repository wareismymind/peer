using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.App;

public interface IServiceSetupHandler
{
    public Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services);
}
