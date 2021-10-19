using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class IdentifierTests
    {
        public class IsMatch
        {
            private readonly Identifier _identifier = new Identifier("10", "secundatives", "wareismymind", "github");

            [Theory]
            [InlineData("github/wareismymind/secundatives/11")]
            [InlineData("github/wareismymind/peer/10")]
            [InlineData("github/dootdoot/secundatives/10")]
            [InlineData("azdo/wareismymind/secundatives/10")]
            public void AnySegmentDoesNotMatch_IsNotMatch(string partial)
            {
                var res = _identifier.IsMatch(partial);

                Assert.True(res.IsValue);
                Assert.False(res.Value);
            }

            [Theory]
            [InlineData("10")]
            [InlineData("secundatives/10")]
            [InlineData("wareismymind/secundatives/10")]
            public void PartialsSubsectionsAllMatch_IsMatch(string partial)
            {
                var res = _identifier.IsMatch(partial);

                Assert.True(res.IsValue);
                Assert.True(res.Value);
            }

            [Fact]
            public void PartialIsEmpty_ReturnsNoSegmentsToMatch()
            {
                var res = _identifier.IsMatch(string.Empty);
                Assert.True(res.IsError);
                Assert.Equal(MatchError.NoSegmentsToMatch, res.Error);
            }

            [Fact]
            public void PartialHasTooManySections_ReturnsTooManySegments()
            {
                var res = _identifier.IsMatch("a/b/c/d/e/f/g/h");

                Assert.True(res.IsError);
                Assert.Equal(MatchError.TooManySegments, res.Error);
            }

            [Fact]
            public void PartialIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => _identifier.IsMatch(null));
            }

            [Fact]
            public void AllSectionsMatch_IsMatch()
            {
                var res = _identifier.IsMatch("github/wareismymind/secundatives/10");

                Assert.True(res.IsValue);
                Assert.True(res.Value);
            }
        }
    }
}
