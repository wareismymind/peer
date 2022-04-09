using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Configuration.CommandConfigs;
using Peer.Domain.Exceptions;
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

        public ShowConfig Config { get; }

        public Show(
            IPullRequestService prService,
            IListFormatter formatter,
            IConsoleWriter writer,
            ShowConfig config,
            ISorter<PullRequest>? sorter = null,
            IEnumerable<IFilter>? filters = null)
        {
            _pullRequestService = prService;
            _formatter = formatter;
            _writer = writer;
            Config = config;
            _sorter = sorter;
            _filters = filters?.ToList() ?? new();
        }

        public async Task<Result<None, ShowError>> ShowAsync(ShowArguments args, CancellationToken token = default)
        {
            using var cts = new CancellationTokenSource();
            token.Register(() => cts.Cancel());
            cts.CancelAfter(Config.TimeoutSeconds);

            var prs = await GetPullRequests(args, cts.Token);
            if (prs.IsError)
            {
                _writer.Clear();
                _writer.Display(new List<string>
                {
                    $"error: failed to fetch pull request info",
                }, token);
                return prs.Error;
            }

            var lines = _formatter.FormatLines(prs.Value).ToList();
            _writer.Display(lines, token);
            return Maybe.None;
        }

        private async Task<Result<IList<PullRequest>, ShowError>> GetPullRequests(ShowArguments args, CancellationToken token)
        {
            try
            {
                var prs = _pullRequestService.FetchAllPullRequests(token);
                prs = _filters.Aggregate(prs, (prs, filter) => filter.Filter(prs));
                return await (_sorter?.Sort(prs) ?? prs).Take(args.Count).ToListAsync(token);
            }
            catch (OperationCanceledException)
            {
                return ShowError.Timeout;
            }
            catch (FetchException)
            {
                return ShowError.Fire;
            }
        }
    }

    public enum ShowError
    {
        Timeout,
        Fire,
    }
}
