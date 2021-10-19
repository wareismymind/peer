namespace Peer.GitHub.GraphQL.ViewerQuery
{
    public static class Query
    {
        public static string Generate()
        {
            return $"query {{ viewer {{ login }} }}";
        }
    }
}
