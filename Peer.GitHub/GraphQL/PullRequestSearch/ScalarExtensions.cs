using System.Collections.Generic;
using System.Linq;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
    public static class ScalarExtensions
    {
        public static bool In<T>(this T target, params T[] options) => target.In(options.AsEnumerable());

        public static bool In<T>(this T target, IEnumerable<T> options) =>
            options.Any(o => EqualityComparer<T>.Default.Equals(o, target));
    }
}
