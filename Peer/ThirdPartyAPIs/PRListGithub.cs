using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peer.ThirdPartyAPIs
{
    class PRListGithub
    {
        public String FetchPRListFromGithub()
        {
            return @"[
            {""Title"": ""Title1"",
            ""Actor"": ""ABC"",
            ""Date"": ""2020-07-20T00:00:00-07:00""},
            {""Title"": ""Title2"",
            ""Actor"": ""XYZ"",
            ""Date"": ""2020-07-20T00:00:00-07:00""}
        ]";
        }
    }
}
