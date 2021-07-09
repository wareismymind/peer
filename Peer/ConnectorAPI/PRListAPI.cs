using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.ThirdPartyAPIs;

namespace Peer.ConnectorApi
{
    public class PRListApi
    {
        private readonly List<IPullRequestSource> _sources;
        List<PullRequest> PRList;

        public PRListApi(IEnumerable<IPullRequestSource> sources)
        {
            _sources = sources.ToList();
            // default constructor
        }

        public async Task<List<PullRequest>> GetPullRequests()
        {
            var tasks = _sources.Select(x => x.FetchPullRequests());
            var prs = await Task.WhenAll(tasks);
            PRList = prs.SelectMany(x => x).ToList();
            return PRList;
        }
    }
}
