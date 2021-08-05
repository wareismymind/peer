using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer.Connectors
{
    public class AzdoPullRequestSource : IPullRequestSource<AzdoPullRequestSource>
    {
        private readonly AppConfig _config;
        private static IClient<UserAzdoClient> _userClient;

        public AzdoPullRequestSource(AppConfig config, IClient<UserAzdoClient> userClient)
        {
            _config = config;
            _userClient = userClient;
        }

        public async Task<IEnumerable<PeerPullRequest>> FetchPullRequestsAsync()
        {

            var userclient = _userClient.CreateClient();
            var azClient = userclient.AzClient;
            var azProjClient = userclient.AzProjClient;

            var projects = await azProjClient.GetProjects();

            var prs = new List<GitPullRequest>();

            foreach (TeamProjectReference project in projects)
            {
                prs.AddRange(await azClient.GetPullRequestsByProjectAsync(project.Id, null));
            }

            var status = prs.Select(x => new PeerPullRequest(
               title: x.Title,
               assignee: x.CreatedBy.DisplayName,
               id: x.PullRequestId
               )).ToList();

            return status;
        }
    }
}
