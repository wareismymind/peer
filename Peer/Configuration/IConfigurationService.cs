using Microsoft.Extensions.DependencyInjection;
using wimm.Secundatives;

namespace Peer.Configuration
{
    public interface IConfigurationService
    {
        Result<None, ConfigError> RegisterProvidersForConfiguration(IServiceCollection services);
    }
}
