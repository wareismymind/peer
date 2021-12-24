using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Parsing;
using Peer.UnitTests.Util;
using Xunit;

namespace Peer.UnitTests
{
    public class SortParserTests
    {
        public class ParseSortOption
        {

            [Fact]
            public void SortOptionNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => SortParser.ParseSortOption(null));
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData("\t")]
            [InlineData("\r\n")]
            [InlineData("\n")]
            public void SortOptionEmptyOrWhitespace_ReturnsNotEnoughSections(string value)
            {
                var res = SortParser.ParseSortOption(value);
                ResultAsserts.IsError(res, ParseError.NotEnoughSections);
            }

            [Fact]
            public void SortSectionsEmpty_ReturnsNotEnoughSections()
            {
                var res = SortParser.ParseSortOption(":");
                ResultAsserts.IsError(res, ParseError.NotEnoughSections);
            }

            [Fact]
            public void SortOptionHasTooManySections_ReturnsTooManySections()
            {
                var res = SortParser.ParseSortOption(":");
                ResultAsserts.IsError(res, ParseError.NotEnoughSections);
            }

            [Fact]
            public async Task SortDirectionUnspecified_ReturnsAscendingSorter()
            {
                var res = SortParser.ParseSortOption("id");

                ResultAsserts.IsValue(res);

                var prs = new List<PullRequest>()
                {
                    ValueGenerators.CreatePullRequest(),
                    ValueGenerators.CreatePullRequest(),
                    ValueGenerators.CreatePullRequest()
                };

                var reversed = prs.OrderByDescending(p => int.Parse(p.Id)).ToList();
                var sorted = await res.Value.Sort(reversed.ToAsyncEnumerable()).ToListAsync();

                Assert.Equal(reversed.AsEnumerable().Reverse(), sorted);
            }

            [Fact]
            public void PropertyNotAvailableForSorting_ReturnsUnknownSortKey()
            {
                var res = SortParser.ParseSortOption("floop:asc");
                ResultAsserts.IsError(res, ParseError.UnknownSortKey);
            }

            [Fact]
            public void DirectionInvalid_ReturnsInvalidSortDirection()
            {
                var res = SortParser.ParseSortOption("id:floop");
                ResultAsserts.IsError(res, ParseError.InvalidSortDirection);
            }

            [Fact]
            public void ParamsValid_ReturnsSuccess()
            {
                var res = SortParser.ParseSortOption("id:asc");
                ResultAsserts.IsValue(res);
            }
        }
    }
}
