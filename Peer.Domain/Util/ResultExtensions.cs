using wimm.Secundatives;

namespace Peer.Domain.Util;

public static class ResultExtensions
{
    public static Result<T, TErr> ValueOr<T, TErr>(this T? value, TErr err)
    {
        if (value == null)
        {
            return err;
        }

        return value;
    }
}
