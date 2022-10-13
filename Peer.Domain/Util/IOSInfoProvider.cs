using System.Runtime.InteropServices;
using wimm.Secundatives;

namespace Peer.Domain.Util
{
    public interface IOSInfoProvider
    {
        Maybe<OSPlatform> GetPlatform();
    }
}
