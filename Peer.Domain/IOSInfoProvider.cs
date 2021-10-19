using System.Runtime.InteropServices;
using wimm.Secundatives;

namespace Peer.Domain
{
    public interface IOSInfoProvider
    {
        public Maybe<OSPlatform> GetPlatform();
    }
}
