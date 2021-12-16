using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Exceptions;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Open
    {
        private readonly IPullRequestService _prService;
        private readonly IOSInfoProvider _infoProvider;

        public Open(IPullRequestService prService, IOSInfoProvider infoProvider)
        {
            _prService = prService;
            _infoProvider = infoProvider;
        }

        public async Task<Result<None, OpenError>> OpenAsync(OpenArguments openOptions, CancellationToken token = default)
        {
            var res = await _prService.FindByPartial(openOptions.Partial, token)
                .MapError(err => err switch
                {
                    FindError.AmbiguousMatch => OpenError.AmbiguousPattern,
                    FindError.NotFound => OpenError.NotFound,
                    _ => throw new UnreachableException()
                })
                .Map(pr => OpenUrl(pr.Url).OkOr(OpenError.FailedToOpen));

            return res.Map(_ => Maybe.None);
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
    }

    public enum OpenError
    {
        Fire,
        AmbiguousPattern,
        FailedToOpen,
        NotFound
    }
}
