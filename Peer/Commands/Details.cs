using System.Threading;
using System.Threading.Tasks;
using Peer.Domain;
using wimm.Secundatives;

namespace Peer.Commands
{
    public class Details(
        IPullRequestService prService,
        IDetailsFormatter formatter,
        IConsoleWriter consoleWriter)
    {
        private readonly IPullRequestService _prService = prService;
        private readonly IDetailsFormatter _formatter = formatter;
        private readonly IConsoleWriter _consoleWriter = consoleWriter;

        public async Task<Result<None, FindError>> DetailsAsync(DetailsArguments args, CancellationToken token = default)
        {
            var findResult = await _prService.FindSingleByPartial(args.Partial, token);
            var formatted = findResult.Map(pr => _formatter.Format(pr));

            if (formatted.IsError)
            {
                return formatted.Error;
            }

            _consoleWriter.Display(formatted.Value, token);

            return Maybe.None;
        }
    }
}
