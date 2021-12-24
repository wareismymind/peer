using System;
using System.Collections.Generic;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
    public class SearchParams
    {
        public string Subject { get; }

        public IEnumerable<string> Orgs { get; }

        public IEnumerable<string> ExcludedOrgs { get; }

        public int PageSize { get; }

        public string? After { get; }

        public SearchParams(
            string subject,
            IEnumerable<string> orgs,
            IEnumerable<string> excludedOrgs,
            int pageSize,
            string? endCursor = null)
        {
            // todo: Something should validate that the involves and org tokens don't contain spaces
            // (or that they CAN and we should quote them). I'm torn about whether that's here or
            // in the config parsing or both.

            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Orgs = orgs ?? throw new ArgumentNullException(nameof(orgs));
            ExcludedOrgs = excludedOrgs ?? throw new ArgumentNullException(nameof(excludedOrgs));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be between 1 and 100");

            PageSize = pageSize;
            After = endCursor;
        }
    }
}
