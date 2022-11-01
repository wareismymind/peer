using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Peer.Domain.Util;
using wimm.Secundatives;
using wimm.Secundatives.Extensions;

namespace Peer.Domain.Commands;

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
        var os = _infoProvider.GetPlatform().OkOr(ConfigEditError.UnsupportedOs);

        return os.Map(os =>
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = split[0],
                    Arguments = string.Join(' ', split.Skip(1).Append(path)),
                    UseShellExecute = os == OSPlatform.Windows
                })
                .ValueOr(ConfigEditError.ProcessFailedToOpen))
            .Flatten();
    }

    private Result<Process, ConfigEditError> OpenWithOsDefault(string path)
    {
        //info(cn): See https://github.com/dotnet/runtime/issues/17938
        return _infoProvider.GetPlatform()
            .OkOr(ConfigEditError.UnsupportedOs)
            .Map(
                os => os switch
                {
                    _ when os == OSPlatform.Windows => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = path }).ValueOr(ConfigEditError.ProcessFailedToOpen),
                    _ when os == OSPlatform.OSX => Process.Start("open", path).ValueOr(ConfigEditError.ProcessFailedToOpen),
                    _ => new Result<Process, ConfigEditError>(ConfigEditError.UnsupportedOs)
                })
            .Flatten();
    }
}
