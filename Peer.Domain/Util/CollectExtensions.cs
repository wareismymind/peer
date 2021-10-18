using System.Collections.Generic;
using System.Linq;
using wimm.Secundatives;

namespace Peer.Domain.Util
{
    public static class CollectExtensions
    {

        public static Result<IEnumerable<TValue>, TError> Collect<TValue, TError>(this IEnumerable<Result<TValue, TError>> results)
        {
            var values = new List<TValue>();
            foreach (var result in results)
            {
                if (result.IsError)
                {
                    return result.Error;
                }

                values.Add(result.Value);
            }

            return values;
        }

        public static Result<None, TError> Collect<TError>(this IEnumerable<Result<None, TError>> results)
        {
            return results.FirstOrDefault(x => x.IsError) ?? Maybe.None;
        }
    }
}
