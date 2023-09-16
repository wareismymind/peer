using Peer.Domain;
using wimm.Secundatives;

namespace Peer
{
    public interface ISymbolProvider
    {
        public string GetSymbol(PullRequestStatus status);
    }

    public interface ICheckSymbolProvider
    {
        public Maybe<string> GetSymbol(CheckStatus status, CheckResult result);
    }
}
