using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Exceptions;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Domain.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Dictionary<string, IRegistrationHandler> _handlers;


        public ConfigurationService(IEnumerable<IRegistrationHandler> handlers)
        {
            _handlers = handlers.ToDictionary(x => x.ProviderKey, x => x);
        }

        public Result<None, ConfigError> RegisterProvidersForConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            var providerTypes = configuration.GetSection("Providers").GetChildren();

            Console.WriteLine(configuration.GetSection("Providers").Value);
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

                return handler.Register(providerConfig)
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
