using System.IO.Pipes;
using System.Security.Principal;
using Grpc.Net.Client;
using Peerless;

namespace Geas;

internal class Program
{
    public string[] _commands = { "prs" };
    public string[] _modes = { "unix", "windows" };
    static async Task Main(string[] args)
    {
        var factory = new NamedPipeConnectionFactory("peerless");

        var handler = new SocketsHttpHandler()
        {
            ConnectCallback = factory.ConnectAsync
        };

        var channel = GrpcChannel.ForAddress("http://pipe:/peerless", new GrpcChannelOptions
        {
            HttpHandler = handler
        });

        var client = new PullRequestGrpcService.PullRequestGrpcServiceClient(channel);
        var res = client.GetPullRequest(new GetPullRequestRequest { Id = 1 });
        await Console.Out.WriteLineAsync($"Found: {res.Value.Id}");
    }
}

public class NamedPipeConnectionFactory
{
    private readonly string _pipeName;

    public NamedPipeConnectionFactory(string pipeName)
    {
        _pipeName = pipeName;
    }

    public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
        CancellationToken cancellationToken = default)
    {
        var clientStream = new NamedPipeClientStream(
            serverName: ".",
            pipeName: _pipeName,
            direction: PipeDirection.InOut,
            options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
            impersonationLevel: TokenImpersonationLevel.Anonymous);

        try
        {
            await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return clientStream;
        }
        catch
        {
            clientStream.Dispose();
            throw;
        }
    }
}
