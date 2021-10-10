using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class GitInfoTests
    {
        public class Construct
        {
            private const string _sourceRef = "refs/heads/feat-10";
            private const string _targetRef = "refs/heads/master";

            [Fact]
            public void SourceIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new GitInfo(null, _targetRef));
            }

            [Fact]
            public void SourceIsEmpty_Throws()
            {
                Assert.Throws<ArgumentException>(() => new GitInfo(string.Empty, _targetRef));
            }

            [Fact]
            public void SourceIsWhitespace_Throws()
            {
                Assert.Throws<ArgumentException>(() => new GitInfo("\t\r\n  ", _targetRef));
            }

            [Fact]
            public void TargetIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new GitInfo(_sourceRef, null));
            }

            [Fact]
            public void TargetIsEmpty_Throws()
            {
                Assert.Throws<ArgumentException>(() => new GitInfo(_sourceRef, string.Empty));
            }

            [Fact]
            public void TargetIsWhitespace_Throws()
            {
                Assert.Throws<ArgumentException>(() => new GitInfo(_sourceRef, "\t\r\n  "));
            }

            [Fact]
            public void ValidArgs_Constructs()
            {
                _ = new GitInfo(_sourceRef, _targetRef);
            }
        }
    }
}
