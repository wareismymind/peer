using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Exceptions;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Dictionary<string, IRegistrationHandler> _handlers;
        private readonly IConfiguration _config;

        public ConfigurationService(IEnumerable<IRegistrationHandler> handlers, IConfiguration config)
        {
            _handlers = handlers.ToDictionary(x => x.ProviderKey, x => x);
            _config = config;
        }

        public Result<None, ConfigError> RegisterProvidersForConfiguration(IServiceCollection services)
        {
            var providerTypes = _config.GetSection("Providers").GetChildren();

            if (!providerTypes.Any())
            {
                return ConfigError.NoProvidersConfigured;
            }

            return providerTypes.Select(providerConfig =>
            {
                if (!_handlers.TryGetValue(providerConfig.Key, out var handler))
                {
                    return ConfigError.ProviderNotMatched;
                }

                return handler.Register(providerConfig, services)
                    .MapError(err => err switch
                    {
                        RegistrationError.BadConfig => ConfigError.InvalidProviderValues,
                        RegistrationError.ProviderMismatch => throw new InvalidOperationException("Programming error - handler was actioned with improper provider type"),
                        _ => throw new UnreachableException()
                    });
            }).Collect();
        }
    }
}
