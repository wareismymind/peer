using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Open
    {
        private readonly List<IPullRequestFetcher> _fetchers;
        private readonly IOSInfoProvider _infoProvider;

        public Open(IEnumerable<IPullRequestFetcher> fetchers,IOSInfoProvider infoProvider)
        {
            _fetchers = fetchers.ToList();
            _infoProvider = infoProvider;
        }

        public async Task<Result<None, OpenError>> OpenAsync(OpenConfig openOptions, CancellationToken token = default)
        {
            var prs = await FetchAllSources(token);

            var res = prs.Select(pr =>
            {
                return pr.Identifier.IsMatch(openOptions.Partial)
                    .Map(match => new { Match = match, PullRequest = pr });
            })
            .Collect(); //CN: Being lazy here but we should really have a fn that can handle this kinda thing

            if (res.IsError)
            {
                return res.Error switch
                {
                    MatchError.NoSegmentsToMatch => OpenError.FormatError,
                    MatchError.TooManySegments => OpenError.FormatError,
                    _ => OpenError.Fire
                };
            }

            var matches = res.Value
                .Where(x => x.Match)
                .ToList();

            if (matches.Count > 1)
            {
                return OpenError.AmbiguousPattern;
            }

            return OpenUrl(matches.First().PullRequest.Url)
                .OkOr(OpenError.FailedToOpen)
                .Map(_ => Maybe.None);
        }

        private Maybe<Process?> OpenUrl(Uri url)
        {
            //info(cn): See https://github.com/dotnet/runtime/issues/17938
            return _infoProvider.GetPlatform()
                .Map(os => os switch
                {
                    var x when x == OSPlatform.Windows => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = url.ToString() }),
                    var x when x == OSPlatform.Linux => Process.Start("xdg-open", url.ToString()),
                    var x when x == OSPlatform.OSX => Process.Start("open", url.ToString()),
                    _ => Maybe<Process?>.None
                });
        }

        private async Task<IEnumerable<PullRequest>> FetchAllSources(CancellationToken token)
        {
            var tasks = _fetchers.Select(async x => await x.GetPullRequestsAsync(token));
            var prs = await Task.WhenAll(tasks);
            var combined = prs.SelectMany(x => x);

            return combined;
        }
    }

    public enum OpenError
    {
        Fire,
        FormatError,
        AmbiguousPattern,
        FailedToOpen
    }
}
