namespace Peer.GitHub.GraphQL
{
#nullable disable
    public class PageInfo
    {
        public bool HasNextPage { get; set; }
        public string EndCursor { get; set; }
        public bool HasPreviousPage { get; set; }
        public string StartCursor { get; set; }
    }
#nullable enable
}
