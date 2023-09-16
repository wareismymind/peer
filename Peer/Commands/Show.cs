﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peer.Configuration.CommandConfigs;
using Peer.Domain;
using Peer.Domain.Exceptions;
using Peer.Filters;
using wimm.Secundatives;

namespace Peer.Commands
{
    public class Show(
        IPullRequestService prService,
        IListFormatter formatter,
        IConsoleWriter writer,
        ShowConfig config,
        ISorter<PullRequest>? sorter = null,
        IEnumerable<IFilter>? filters = null)
    {
        private readonly IPullRequestService _pullRequestService = prService;
        private readonly IListFormatter _formatter = formatter;
        private readonly IConsoleWriter _writer = writer;
        private readonly ISorter<PullRequest>? _sorter = sorter;
        private readonly List<IFilter> _filters = filters?.ToList() ?? new();

        public ShowConfig Config { get; } = config;

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
                    prs.Error switch
                    {
                        ShowError.Timeout => "error: timeout fetching pull request info; consider increasing the 'ShowTimeoutSeconds' in your peer config",
                        _ => "error: failed to fetch pull request info",
                    },
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