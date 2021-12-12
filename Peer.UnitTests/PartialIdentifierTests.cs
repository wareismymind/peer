using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class PartialIdentifierTests
    {
        public class Parse
        {
            [Fact]
            public void PartialIsEmpty_ReturnsNoSegmentsToMatch()
            {
                var res = PartialIdentifier.Parse(string.Empty);
                ResultAsserts.IsError(res, ParsePartialError.NoSegmentsToMatch);
            }

            [Fact]
            public void PartialHasTooManySections_ReturnsTooManySegments()
            {
                var res = PartialIdentifier.Parse("a/b/c/d/e/f/g/h");
                ResultAsserts.IsError(res, ParsePartialError.TooManySegments);
            }
        }
    }
}
