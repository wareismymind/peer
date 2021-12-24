namespace Peer.Domain.Commands
{
    public class ShowArguments
    {
        public int Count { get; }
        public ShowArguments(int count)
        {
            Count = count;
        }
    }
}
