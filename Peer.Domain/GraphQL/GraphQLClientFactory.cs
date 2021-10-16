using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Client.Http;

namespace Peer.Domain.GraphQL
{
    public class GraphQLClientFactory : IGraphQLClientFactory
    {
        private readonly List<NamedGraphQLBinding> _bindings;

        public GraphQLClientFactory(IEnumerable<NamedGraphQLBinding> bindings)
        {
            _bindings = bindings?.ToList() ?? throw new ArgumentNullException(nameof(bindings));
        }

        public GraphQLHttpClient GetClient(string name)
        {
            return _bindings.FirstOrDefault(x => x.Name == name)
                ?.Client
                ?? throw new ArgumentOutOfRangeException($"Client matching {name} not found");
        }
    }
}
