using Grpc.Core;
using Microsoft.Extensions.Logging;
using Peerless;

namespace Peer.Server.Services;

public class PullRequestGrpc : PullRequestGrpcService.PullRequestGrpcServiceBase
{
    private readonly ILogger<PullRequestGrpc> _logger;

    public PullRequestGrpc(ILogger<PullRequestGrpc> logger)
    {
        _logger = logger;
    }

    public override Task<GetPullRequestResponse> GetPullRequest(GetPullRequestRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetPullRequestResponse()
        {
            Value = new PullRequestModel()
            {
                Id = 100.ToString(),
                Identifier = new IdentifierModel { },
                Url = "wakawka"
            }
        });
    }

    public override Task<ListPullRequestsResponse> ListPullRequests(ListPullRequestsRequest request, ServerCallContext context)
    {
        return base.ListPullRequests(request, context);
    }
}
