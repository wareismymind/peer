using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wimm.Secundatives;

namespace Peer.Domain.Configuration
{
    public interface IRegistrationHandler
    {
        string ProviderKey { get; }
        Result<None, RegistrationError> Register(IConfigurationSection config, IServiceCollection services);
    }
}
