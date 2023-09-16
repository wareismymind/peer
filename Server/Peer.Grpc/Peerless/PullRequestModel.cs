using Peer.Domain;

namespace Peerless;

public partial class PullRequestModel
{
    public PullRequestModel(PullRequest pr)
    {
        Id = pr.Id;
        Url = pr.Url.ToString();
        Identifier = new IdentifierModel(pr.Identifier);
    }
}
