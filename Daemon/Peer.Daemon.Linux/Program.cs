using Microsoft.AspNetCore.Server.Kestrel.Core;
using Peer.Server;

namespace Peer.Daemon.Linux
{
    internal class Program
    {
        private const string _socketPath = "/var/run/peerless/peerless.sock";
        static async Task Main()
        {
            Directory.CreateDirectory(Directory.GetParent(_socketPath)!.FullName);
            File.Delete(_socketPath);

            var app = ServerBuilder.CreateHost(cfg =>
            {
                cfg.WebHost.UseKestrel(kestrel =>
                {
                    kestrel.ListenUnixSocket(_socketPath, opts =>
                    {
                        opts.Protocols = HttpProtocols.Http2;
                    });
                });
            });

            await app.RunAsync();
        }
    }
}
