using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class PullRequestTests
    {
        public class Construct
        {
            private readonly string _id = "doot";
            private readonly Uri _url = new("https://doot.com/pr/1");
            private readonly Descriptor _descriptor = new("title", "description");
            private readonly State _state = new(PullRequestStatus.ActionsPending, 10, 4);
            private readonly GitInfo _gitInfo = new("refs/heads/wat", "refs/head/to");
            private readonly Identifier _identifier = new("1", "peer", "wareismymind", "github");
            [Fact]
            public void IdNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(null, _identifier, _url, _descriptor, _state, _gitInfo));
            }

            [Fact]
            public void IdEmpty_Throws()
            {
                Assert.Throws<ArgumentException>(() => new PullRequest(string.Empty, _identifier, _url, _descriptor, _state, _gitInfo));
            }

            [Fact]
            public void IdentifierNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(_id, null, _url, _descriptor, _state, _gitInfo));
            }

            [Fact]
            public void IdWhitespace_Throws()
            {
                Assert.Throws<ArgumentException>(() => new PullRequest("\t \r\n", _identifier, _url, _descriptor, _state, _gitInfo));
            }

            [Fact]
            public void UrlNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(_id, _identifier, null, _descriptor, _state, _gitInfo));
            }

            [Fact]
            public void DescriptorNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(_id, _identifier, _url, null, _state, _gitInfo));
            }

            [Fact]
            public void StateNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(_id, _identifier, _url, _descriptor, null, _gitInfo));
            }

            [Fact]
            public void GitInfoNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new PullRequest(_id, _identifier, _url, _descriptor, _state, null));
            }

            [Fact]
            public void ArgsValid_Constructs()
            {
                _ = new PullRequest(_id, _identifier, _url, _descriptor, _state, _gitInfo);
            }
        }
    }
}
