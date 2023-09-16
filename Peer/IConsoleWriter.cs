using System.Collections.Generic;
using System.Threading;

namespace Peer
{
    public interface IConsoleWriter
    {
        void Display(IList<string> lines, CancellationToken token);
        void Clear();
    }
}
