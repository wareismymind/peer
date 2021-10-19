using System.Runtime.InteropServices;
using wimm.Secundatives;

namespace Peer.Domain
{
    public interface IOSInfoProvider
    {
        Maybe<OSPlatform> GetPlatform();
    }
}
