using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Models;
using Peer.ThirdPartyAPIs;

namespace Peer.ConnectorAPI
{
    public class PRListAPI
    {
        List<IPRModel> PRList;
        public PRListAPI()
        {
            // default constructor
        }

        public List<IPRModel> GetPRListFromGithub()
        {
            PRList = JsonSerializer.Deserialize<List<IPRModel>>(new PRListGithub().FetchPRListFromGithub());
            return PRList;
        }
    }
}
