using Peer.Domain;

namespace Peerless;

public partial class IdentifierModel
{
    public IdentifierModel(Identifier identifier)
    {
        Id = identifier.Id;
        Repo = identifier.Repo;
        Owner = identifier.Owner;
        Provider = identifier.Provider;
        Author = identifier.Author;
    }
}
