using System;

namespace Peer.Domain
{
    public class State
    {
        public PullRequestStatus Status { get; }
        public int TotalComments { get; }
        public int ActiveComments { get; }

        public State(PullRequestStatus status, int totalComments, int activeComments)
        {
            if (!Enum.IsDefined(status))
            {
                throw new ArgumentException("Enum value was undefined", nameof(status));
            }

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
