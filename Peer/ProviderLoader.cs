using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps.AppBuilder;
using Peer.Configuration;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer;

public class ProviderLoader(IEnumerable<IRegistrationHandler> registrationHandlers) : IRunTimeConfigHandler
{
    private readonly List<IRegistrationHandler> _registrationHandlers = registrationHandlers.ToList();

    public Result<None, ConfigError> ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        var configLoader = new ConfigurationService(_registrationHandlers, config);
        return configLoader.RegisterProvidersForConfiguration(services);
    }
}
