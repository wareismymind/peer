using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain.Models;

namespace Peer.IO
{
    public class WritePullRequestTable
    {
        private List<PeerPullRequest> _prList;

        public WritePullRequestTable(IEnumerable<PeerPullRequest> prList)
        {
            _prList = prList.ToList();
        }

        public void Write()
        {
            Console.WriteLine("----------------------------------------" +
                              "| S.No.  |   Assignee   |     Title    |" +
                              "----------------------------------------");
            int index = 1;
            _prList.ForEach(x =>
            {
                Console.WriteLine($"{index++}\t{x.Assignee}\t{x.Title}");
            });
        }
    }
}
