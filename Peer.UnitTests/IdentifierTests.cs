using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class IdentifierTests
    {
        public class IsMatch
        {
            private readonly Identifier _identifier = new("10", "secundatives", "wareismymind", "github");

            [Theory]
            [InlineData("github/wareismymind/secundatives/11")]
            [InlineData("github/wareismymind/peer/10")]
            [InlineData("github/dootdoot/secundatives/10")]
            [InlineData("azdo/wareismymind/secundatives/10")]
            public void AnySegmentDoesNotMatch_IsNotMatch(string raw)
            {
                var partial = PartialIdentifier.Parse(raw).Value;
                var res = _identifier.IsMatch(partial);

                Assert.False(res);
            }

            [Theory]
            [InlineData("10")]
            [InlineData("secundatives/10")]
            [InlineData("wareismymind/secundatives/10")]
            public void PartialsSubsectionsAllMatch_IsMatch(string raw)
            {
                var partial = PartialIdentifier.Parse(raw).Value;
                var res = _identifier.IsMatch(partial);
                Assert.True(res);
            }

            [Fact]
            public void PartialIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => _identifier.IsMatch(null));
            }

            [Fact]
            public void AllSectionsMatch_IsMatch()
            {
                var partial = PartialIdentifier.Parse("github/wareismymind/secundatives/10").Value;
                var res = _identifier.IsMatch(partial);

                Assert.True(res);
            }

            //CN: Move these to the def of paritals tests
            //[Fact]
            //public void PartialIsEmpty_ReturnsNoSegmentsToMatch()
            //{
            //    var res = _identifier.IsMatch(string.Empty);
            //    Assert.True(res.IsError);
            //    Assert.Equal(MatchError.NoSegmentsToMatch, res.Error);
            //}

            //[Fact]
            //public void PartialHasTooManySections_ReturnsTooManySegments()
            //{
            //    var res = _identifier.IsMatch("a/b/c/d/e/f/g/h");

            //    Assert.True(res.IsError);
            //    Assert.Equal(MatchError.TooManySegments, res.Error);
            //}
        }
    }
}
