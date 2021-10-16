using Microsoft.Extensions.Configuration;
using wimm.Secundatives;

namespace Peer.Domain.Configuration
{
    public interface IRegistrationHandler
    {
        string ProviderKey { get; }
        Result<None, RegistrationError> Register(IConfigurationSection config);
    }
}
