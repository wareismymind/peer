﻿using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class PullRequest
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public Uri Url { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; }
        public string BaseRefName { get; set; }
        public string HeadRefName { get; set; }
        public ReviewThreads ReviewThreads { get; set; }
        public string Mergeable { get; set; }
        public string ReviewDecision { get; set; }
        public bool IsDraft { get; set; }
        public Commits Commits { get; set; }

        public bool ActionsPending =>
            Commits.Nodes[0].Commit.StatusCheckRollup?.State.In(new[] { "PENDING", "EXPECTED" }) ?? false;

        public bool IsStale => CreatedAt < DateTime.UtcNow.AddDays(-14);

        public bool HasFailedChecks => Commits.Nodes[0].Commit.StatusCheckRollup?.State.In("FAILURE", "ERROR") ?? false;

        public bool ReadyToMerge =>
            Mergeable == "MERGEABLE"
                && (Commits.Nodes[0].Commit.StatusCheckRollup?.State.Equals("SUCCESS") ?? true)
                && ReviewDecision.In("APPROVED", null);
        public BaseRepository BaseRepository { get; set; }

        public Domain.PullRequest Into()
        {
            var status = CalculateStatus();
            var totalComments = ReviewThreads.Nodes.Count;
            var activeComments = ReviewThreads.Nodes.Count(t => !t.IsResolved);
            return new Domain.PullRequest(
                Number.ToString(),
                new Identifier(Number.ToString(), BaseRepository.Name, BaseRepository.Owner.Login, ProviderConstants.Github),
                Url,
                new Descriptor(Title, Body ?? string.Empty),
                new State(status, totalComments, activeComments),
                new GitInfo(HeadRefName, BaseRefName));
        }

        private PullRequestStatus CalculateStatus()
        {
            return this switch
            {
                { Mergeable: "UNKNOWN" } => PullRequestStatus.Unknown,
                { Mergeable: "CONFLICTING" } => PullRequestStatus.Conflict,
                { IsDraft: true } => PullRequestStatus.Draft,
                { ActionsPending: true } => PullRequestStatus.ActionsPending,
                { IsStale: true } => PullRequestStatus.Stale,
                { HasFailedChecks: true } => PullRequestStatus.FailedChecks,
                { ReviewDecision: "REVIEW_REQUIRED" } => PullRequestStatus.AwaitingReview,
                { ReviewDecision: "CHANGES_REQUESTED" } => PullRequestStatus.FixesRequested,
                { ReadyToMerge: true } => PullRequestStatus.ReadyToMerge,
                _ => PullRequestStatus.Unknown,
            };
        }
    }

    public static class ScalarExtensions
    {
        public static bool In<T>(this T target, params T[] options) => target.In(options.AsEnumerable());

        public static bool In<T>(this T target, IEnumerable<T> options) =>
            options.Any(o => EqualityComparer<T>.Default.Equals(o, target));
    }
#nullable enable
}