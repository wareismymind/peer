using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wimm.Secundatives;

namespace Peer.Domain.Configuration
{
    public interface IConfigurationService
    {
        Result<None, ConfigError> RegisterProvidersForConfiguration(IServiceCollection services);
    }
}
