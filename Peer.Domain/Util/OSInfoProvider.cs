using System.Runtime.InteropServices;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class OSInfoProvider : IOSInfoProvider
    {
        public Maybe<OSPlatform> GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            //TODO:CN -- Maybe a result but seems like "Couldn't find your OS" is valid
            return Maybe.None;
        }
    }
}
