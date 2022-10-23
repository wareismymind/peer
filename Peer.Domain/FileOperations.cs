using System.IO;

namespace Peer.Domain;

public class FileOperations : IFileOperations
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public FileStream Create(string path)
    {
        return File.Create(path);
    }
}
