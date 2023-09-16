using Peer.Domain;

namespace Peer.Commands
{
    public class DetailsArguments
    {
        public PartialIdentifier Partial { get; }

        public DetailsArguments(PartialIdentifier partial)
        {
            Partial = partial;
        }
    }
}
