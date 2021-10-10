using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain;
using Peer.Domain.Formatters;
using Peer.UnitTests.Util;
using Xunit;

namespace Peer.UnitTests.Formatters
{
    public class CompactFormatterTests
    {
        public class FormatLines
        {
            [Fact]
            public void PullRequestsNull_Throws()
            {
                var underTest = Construct();
                Assert.Throws<ArgumentNullException>(() => underTest.FormatLines(null).ToList());
            }

            [Fact]
            public void PullRequestsEmpty_ReturnsOnlyHeader()
            {
                var underTest = Construct();
                var res = underTest.FormatLines(new List<PullRequest>());

                Assert.Single(res);
            }

            [Fact]
            public void PullRequestsExist_ReturnsPullRequestsAndHeader()
            {
                var underTest = Construct();
                var prs = Enumerable.Range(0, 10).Select(_ => ValueGenerators.CreatePullRequest());
                var res = underTest.FormatLines(prs);
                Assert.Equal(11, res.Count());
            }

            private static CompactFormatter Construct()
            {
                return new CompactFormatter(new DefaultEmojiProvider());
            }
        }
    }
}
