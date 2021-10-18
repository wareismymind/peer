using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Commands;
using wimm.Secundatives;

namespace Peer.Domain
{
    public interface IPeerApplication
    {
        Task<Result<None, ShowError>> ShowAsync(Show showOptions, CancellationToken token = default);
    }

    public enum ShowError
    {
        Fire,
    }

    public enum OpenError
    {
        Fire
    }
}
