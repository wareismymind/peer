using System;
using Peer.Domain.Util;

namespace Peer.Domain
{
    public class State
    {
        public PullRequestStatus Status { get; }
        public int TotalComments { get; }
        public int ActiveComments { get; }

        public State(PullRequestStatus status, int totalComments, int activeComments)
        {
            Validators.ArgIsDefined(status, nameof(status));

            if (activeComments > totalComments)
            {
                throw new ArgumentException("Active comments cannot be greater than total comments. That isn't how totals work :(");
            }

            Status = status;
            TotalComments = totalComments;
            ActiveComments = activeComments;
        }
    }
}
