using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Peer.Utils
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> fn) : base(async () =>  await fn()) { }

        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}
