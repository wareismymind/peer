using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain.Exceptions;

namespace Peer.Domain
{
    public interface ISorter<T>
    {
        IEnumerable<T> Sort(IEnumerable<T> input);
    }

    public class SelectorSorter<T> : ISorter<T>
    {
        private readonly Func<T, IComparable> _selector;
        private readonly SortDirection _direction;

        public SelectorSorter(Func<T, IComparable> selector, SortDirection direction)
        {
            _selector = selector;
            _direction = direction;
        }

        public IEnumerable<T> Sort(IEnumerable<T> input)
        {
            return _direction switch
            {
                SortDirection.Ascending => input.OrderBy(_selector),
                SortDirection.Descending => input.OrderByDescending(_selector),
                _ => throw new UnreachableException()
            };
        }
    }

    public enum SortDirection
    {
        Ascending,
        Descending,
    }
}
