using System;
using GraphQL.Client.Http;

namespace Peer.Domain.GraphQL
{
    public sealed class NamedGraphQLBinding : IDisposable
    {
        public string Name { get; }
        public GraphQLHttpClient Client { get; }

        public NamedGraphQLBinding(string name, GraphQLHttpClient client)
        {

            Name = name;
            Client = client;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
