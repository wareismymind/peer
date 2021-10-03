namespace Peer.Domain;

public interface ISymbolProvider
{
    public string GetSymbol(PullRequestStatus status);
}
