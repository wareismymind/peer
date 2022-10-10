using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Peer.App.AppBuilder;

namespace Peer.App;

public class App
{
    private readonly CommandLineParser _parser;
    private readonly List<IVerb> _verbs;
    private readonly IServiceSetupHandler? _setupHandler;

    public App(CommandLineParser parser, IEnumerable<IVerb> verbs, IServiceSetupHandler? setupHandler = null)
    {
        _parser = parser;
        _verbs = verbs.ToList();
        _setupHandler = setupHandler;

    }

    public async Task RunAsync(string[] args, CancellationToken token = default)
    {
        var result = _parser.Parse(args);

        var verb = _verbs.SelectMany(x => x.Subs.Append(x)).FirstOrDefault(x => x.Type == result.TypeInfo.Current);

        if (verb == null)
        {
            await WriteHelpText(null, result);
            return;
        }
        var services = new ServiceCollection();
        _setupHandler?.SetupServices(services);


        await result.MapResult<object, Task>(
            x => verb.Handler?.HandleAsync(x, services, token) ?? Task.CompletedTask,
            _ => WriteHelpText(verb, result));
    }

    private static Task WriteHelpText(IVerb? verb, ParserResult<object> result)
    {
        var help = verb?.CustomHelp?.GetHelpText(result) ?? HelpText.AutoBuild(result);
        Console.Write(help);
        return Task.CompletedTask;
    }
}
