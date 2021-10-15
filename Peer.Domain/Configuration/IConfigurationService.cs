using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using wimm.Secundatives;

namespace Peer.Domain.Configuration
{
    public interface IConfigurationService
    {
        Result<None, ConfigError> RegisterProvidersForConfiguration(IConfiguration configuration, IServiceCollection services);
    }
}
