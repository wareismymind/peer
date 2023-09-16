using Microsoft.AspNetCore.Server.Kestrel.Core;
using Peer.Server;

namespace Peer.Daemon.Windows;

public class Program
{
    public async static Task Main(string[] args)
    {
        var app = ServerBuilder.CreateHost(builder =>
        {
            builder.WebHost.UseKestrel(x =>
            {
                x.ListenNamedPipe("peerless", opts =>
                {
                    opts.Protocols = HttpProtocols.Http2;
                });
            });
        });

        await app.RunAsync();
    }
}
