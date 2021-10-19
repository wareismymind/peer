using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Commands;
using Peer.Domain.Exceptions;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class PeerApplication : IPeerApplication
    {
        private readonly List<IPullRequestFetcher> _fetchers;
        private readonly IPullRequestFormatter _formatter;
        private readonly IOSInfoProvider _infoProvider;
        private readonly IConsoleWriter _writer;

        public PeerApplication(
            IEnumerable<IPullRequestFetcher> fetchers,
            IPullRequestFormatter formatter,
            IOSInfoProvider infoProvider,
            IConsoleWriter writer)
        {
            _fetchers = fetchers.ToList();
            _formatter = formatter;
            _infoProvider = infoProvider;
            _writer = writer;
        }

        public async Task<Result<None, ShowError>> ShowAsync(Show showOptions, CancellationToken token = default)
        {
            var prs = await FetchAllSources(token);
            _writer.Display(_formatter.FormatLines(prs).ToList(), token);
            return Maybe.None;
        }

        public async Task<Result<None, OpenError>> OpenAsync(Open openOptions, CancellationToken token = default)
        {
            var prs = await FetchAllSources(token);

            var res = prs.Select(pr => pr.Identifier.IsMatch(openOptions.Partial)
                .Map(match => new { Match = match, PullRequest = pr }))
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

            if (!OpenUrl(matches.First().PullRequest.Url).Exists)
            {
                return OpenError.FailedToOpen;
            }

            return Maybe.None;
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
                    _ => throw new UnreachableException()
                });
        }

        private async Task<IEnumerable<PullRequest>> FetchAllSources(CancellationToken token)
        {
            var tasks = _fetchers.Select(async x => await x.GetPullRequestsAsync());
            var prs = await Task.WhenAll(tasks);
            var combined = prs.SelectMany(x => x);

            return combined;
        }
    }

    public interface IOSInfoProvider
    {
        public Maybe<OSPlatform> GetPlatform();
    }

    public class OSInfoProvider : IOSInfoProvider
    {
        public Maybe<OSPlatform> GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            //TODO:CN -- Maybe a result but seems like "Couldn't find your OS" is valid
            return Maybe.None;
        }
    }
}
