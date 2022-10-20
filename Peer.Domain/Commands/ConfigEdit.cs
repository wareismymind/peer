using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Peer.Domain.Util;
using wimm.Secundatives;
using wimm.Secundatives.Extensions;

namespace Peer.Domain.Commands;

public interface IFileOperations
{
    bool Exists(string path);
    FileStream Create(string path);
}

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

public class ConfigEditConfig
{
    public string? Editor { get; }
    public string ConfigPath { get; }

    public ConfigEditConfig(string? editor, string configPath)
    {
        Editor = editor;
        ConfigPath = configPath;
    }
}

public class ConfigEdit
{
    private readonly IOSInfoProvider _infoProvider;
    private readonly ConfigEditConfig _config;
    private readonly IFileOperations _fileOps;

    public ConfigEdit(ConfigEditConfig config, IOSInfoProvider infoProvider, IFileOperations fileOps)
    {
        _config = config;
        _infoProvider = infoProvider;
        _fileOps = fileOps;
    }

    public Task<Result<None, ConfigEditError>> RunAsync()
    {
        return OpenFileAsync(_config.ConfigPath);
    }

    private async Task<Result<None, ConfigEditError>> OpenFileAsync(string path)
    {
        if (!_fileOps.Exists(path))
        {
            await using var _ = File.Create(path);
        }

        var proc = _config.Editor == null
            ? OpenWithOsDefault(path)
            : OpenWithEditor(path);


        if (proc.IsError)
        {
            return proc.Error;
        }

        await proc.Value!.WaitForExitAsync();
        return Maybe.None;
    }

    private Result<Process, ConfigEditError> OpenWithEditor(string path)
    {
        var split = _config.Editor!.Split(' ').ToArray();
        return Process.Start(split[0], split[1..].Append(path));
    }
    
    
    private Result<Process, ConfigEditError> OpenWithOsDefault(string path)
    {
        //info(cn): See https://github.com/dotnet/runtime/issues/17938
        return _infoProvider.GetPlatform()
            .OkOr(ConfigEditError.UnsupportedOs)
            .Map(
                os => os switch
                {
                    _ when os == OSPlatform.Windows => Process.Start(
                            new ProcessStartInfo { UseShellExecute = true, FileName = path })!.AsMaybe()
                        .OkOr(() => ConfigEditError.ProcessFailedToOpen),
                    _ when os == OSPlatform.Linux => new Result<Process, ConfigEditError>(
                        Process.Start("xdg-open", path)),
                    _ when os == OSPlatform.OSX => new Result<Process, ConfigEditError>(Process.Start("open", path)),
                    _ => new Result<Process, ConfigEditError>(ConfigEditError.UnsupportedOs)
                })
            .Flatten();
    }
}

public enum ConfigEditError
{
    Fire,
    UnsupportedOs,
    ProcessFailedToOpen,
}
