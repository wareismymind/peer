using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class DescriptorTests
    {
        public class Construct
        {
            private const string _title = "title";
            private const string _description = "description";

            [Fact]
            public void TitleNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new Descriptor(null, _description));
            }

            [Fact]
            public void TitleEmpty_Throws()
            {
                Assert.Throws<ArgumentException>(() => new Descriptor(string.Empty, _description));
            }

            [Fact]
            public void TitleWhitespace_Throws()
            {
                Assert.Throws<ArgumentException>(() => new Descriptor(" \t\r\n", _description));
            }

            [Fact]
            public void DescriptionNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new Descriptor(_title, null));
            }

            [Fact]
            public void ArgsValid_Constructs()
            {
                _ = new Descriptor(_title, _description);
            }
        }
    }
}
