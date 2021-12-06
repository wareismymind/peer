using System;
using System.Collections.Generic;
using Peer.Domain;
using Peer.Domain.Exceptions;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class CommitNode
    {
        public Commit Commit { get; set; }
    }

    public class NodeList<T>
    {
        public List<T> Nodes { get; set; }
    }

    public class CheckRun
    {
        public string Name { get; set; }
        public CheckConclusionState Conclusion { get; set; }
        public Uri Url { get; set; }
        public CheckRunStatusState Status { get; set; }

        public Check Into()
        {
            return new Check(Name, null, Url, MapStatus(), MapResult());
        }

        private CheckStatus MapStatus()
        {
            return Status switch
            {
                CheckRunStatusState.Queued => CheckStatus.Queued,
                CheckRunStatusState.In_Progress => CheckStatus.InProgress,
                CheckRunStatusState.Waiting => CheckStatus.Waiting,
                CheckRunStatusState.Requested => CheckStatus.Requested,
                CheckRunStatusState.Pending => CheckStatus.PendingExecution,
                CheckRunStatusState.Completed => CheckStatus.Completed,
                _ => throw new UnreachableException()
            };
        }

        private CheckResult MapResult()
        {
            return Conclusion switch
            {
                CheckConclusionState.Success => CheckResult.Success,
                CheckConclusionState.Action_Required => CheckResult.ActionRequired,
                CheckConclusionState.Cancelled => CheckResult.Cancelled,
                CheckConclusionState.Failure => CheckResult.Failure,
                CheckConclusionState.Neutral => CheckResult.Neutral,
                CheckConclusionState.Skipped => CheckResult.Skipped,
                CheckConclusionState.Stale => CheckResult.Stale,
                CheckConclusionState.Startup_Failure => CheckResult.Fire,
                CheckConclusionState.Timed_Out => CheckResult.Timeout,
                _ => throw new UnreachableException()
            };
        }
    }

    public enum CheckRunStatusState
    {
        Queued,
        In_Progress,
        Completed,
        Waiting,
        Pending,
        Requested
    }


    public enum CheckConclusionState
    {
        Action_Required,
        Cancelled,
        Failure,
        Neutral,
        Skipped,
        Stale,
        Startup_Failure,
        Success,
        Timed_Out
    }
#nullable enable
}
