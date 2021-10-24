using wimm.Secundatives;
using Xunit;

namespace Peer.UnitTests
{
    public static class ResultAsserts
    {
        public static void IsError<T, TErr>(Result<T, TErr> result)
        {
            Assert.True(result.IsError, "Result was not an error");
        }

        public static void IsError<T, TErr>(Result<T, TErr> result, TErr value)
        {
            IsError(result);
            Assert.Equal(value, result.Error);
        }

        public static void IsValue<T, TErr>(Result<T, TErr> result)
        {
            Assert.True(result.IsValue, "Result was not a value");
        }

        public static void IsValue<T, TErr>(Result<T, TErr> result, T value)
        {
            IsValue(result);
            Assert.Equal(value, result);
        }
    }
}
