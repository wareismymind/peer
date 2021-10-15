using GraphQL.Client.Http;

namespace Peer.Domain.GraphQL
{
    public interface IGraphQLClientFactory
    {
        public GraphQLHttpClient GetClient(string name);
    }
}
