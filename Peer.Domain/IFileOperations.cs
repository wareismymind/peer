using System.IO;

namespace Peer.Domain;

public interface IFileOperations
{
    bool Exists(string path);
    FileStream Create(string path);
}