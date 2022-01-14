using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Filters;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Show
    {
        private readonly IPullRequestService _pullRequestService;
        private readonly IListFormatter _formatter;
        private readonly IConsoleWriter _writer;
        private readonly ISorter<PullRequest>? _sorter;
        private readonly List<IFilter> _filters;

        public Show(
            IPullRequestService prService,
            IListFormatter formatter,
            IConsoleWriter writer,
            ISorter<PullRequest>? sorter = null,
            IEnumerable<IFilter>? filters = null)
        {
            _pullRequestService = prService;
            _formatter = formatter;
            _writer = writer;
            _sorter = sorter;
            _filters = filters?.ToList() ?? new();
        }

        public async Task<Result<None, ShowError>> ShowAsync(ShowArguments args, CancellationToken token = default)
        {
            try
            {
                var prs = await _pullRequestService.FetchAllPullRequests(token);
                prs = _filters.Aggregate(prs, (prs, filter) => filter.Filter(prs));

                var sorted = await (_sorter?.Sort(prs) ?? prs).Take(args.Count).ToListAsync(token);
                var lines = _formatter.FormatLines(sorted).ToList();
                _writer.Display(lines, token);
                return Maybe.None;
            }
            catch (Exception ex)
            {
                _writer.Clear();
                _writer.Display(new List<string>
                {
                    $"error: failed to fetch pull request info: {ex.Message}",
                }, token);
                return ShowError.Fire;
            }
        }
    }

    public enum ShowError
    {
        Fire,
    }
}
