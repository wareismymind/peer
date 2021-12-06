namespace Peer.Domain.Commands
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
