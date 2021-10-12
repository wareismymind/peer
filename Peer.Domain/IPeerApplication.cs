using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain
{
    public interface IPeerApplication
    {
        Task<Result<None, ShowError>> ShowAsync();
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
