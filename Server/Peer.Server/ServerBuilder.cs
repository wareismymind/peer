using Grpc.Core.Interceptors;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Peer.Server.Persistence;
using static Peerless.PullRequestGrpcService;
using Peer.Server.Services;

namespace Peer.Server
{
    public class ServerBuilder
    {
        //private const string _socketPath = "/var/run/peerless/peerless.sock";
        public static WebApplication CreateHost(Action<WebApplicationBuilder> config)
        {
            var builder = WebApplication.CreateBuilder();
            config.Invoke(builder);
            builder.Services.AddLogging(c =>
            {
                c.ClearProviders();
                c.AddConsole();
                c.SetMinimumLevel(LogLevel.Trace);
            });

            builder.Services.AddHostedService<PeerSyncWorker>();
            builder.Services.AddGrpc();
            RegisterPersistence(builder.Services);

            var host = builder.Build();
            host.MapGrpcService<PullRequestGrpc>();
            var src = host.Services.GetRequiredService<EndpointDataSource>();
            return host;
        }

        public static IServiceCollection RegisterPersistence(IServiceCollection services)
        {
            services.AddDbContext<PeerContext>(x => x.UseSqlite($"Data Source={SpecialPaths.StoragePath}"));
            return services;
        }
    }

    public class PeerServerConfig
    {
        public string? StoragePath { get; set; }
    }

    //Linux
    public partial class SpecialPaths
    {
        public const string ConfigPath = "/etc/peer/peer.config";
        public const string StoragePath = "/etc/peer/peer.sqlitedb";
    }
}

