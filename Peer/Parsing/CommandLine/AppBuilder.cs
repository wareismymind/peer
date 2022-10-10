using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Parsing.CommandLine;

public class AppBuilder
{
    private readonly IServiceCollection _services;

    public AppBuilder(IServiceCollection services)
    {
        _services = services;
        _services.AddSingleton<CommandLineParser>();
        _services.AddSingleton<App>();
    }

    public AppBuilder WithParseTimeServiceConfig(
        Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        config.Invoke(_services);
        return this;
    }

    public AppBuilder WithSharedServiceConfig(Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        _services.AddSingleton<IServiceSetupHandler>(new FuncServiceSetupHandler(config));
        return this;
    }

    public VerbBuilder<TVerb> WithVerb<TVerb>()
    {
        //TODO - Check TVerb for verby-ness
        _services.AddSingleton<IVerb, Verb<TVerb>>();
        return new VerbBuilder<TVerb>(_services);
    }

    public App Build()
    {
        var sp = _services.BuildServiceProvider();
        return sp.GetRequiredService<App>();
    }

}

public interface IServiceSetupHandler
{
    public Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services);
}

//CN -- Mostly just a temp measure until it gets moved into a type
public class FuncServiceSetupHandler : IServiceSetupHandler
{
    private readonly Func<IServiceCollection, Result<IServiceCollection, ConfigError>> _config;

    public FuncServiceSetupHandler(Func<IServiceCollection, Result<IServiceCollection, ConfigError>> config)
    {
        _config = config;
    }

    public Result<IServiceCollection, ConfigError> SetupServices(IServiceCollection services)
    {
        return _config(services);
    }
}

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

public class CommandLineParser
{
    private readonly List<IVerb> _verbs;

    public CommandLineParser(IEnumerable<IVerb> verbs)
    {
        _verbs = verbs.ToList();
    }

    public ParserResult<object> Parse(string[] args)
    {
        //I'm just dealing with one layer of nesting right now. I'm lazy
        var noDash = args.Count(x => !x.StartsWith('-'));
        var topLevel = _verbs.FirstOrDefault(x => x.Name == args.FirstOrDefault());
        var subs = topLevel?.Subs.ToArray() ?? Array.Empty<IVerb>();
        var parser = new Parser(config =>
        {
            config.AutoHelp = true;
            config.AutoVersion = true;
            config.IgnoreUnknownArguments = false;
        });


        if (noDash == 1 || topLevel == null)
        {
            //CN - Being sneaky here
            if (subs.Any() && topLevel?.Handler == null)
            {
                return parser.ParseArguments(args[1..], subs.Select(x => x.Type).ToArray());
            }

            return parser.ParseArguments(args, _verbs.Select(x => x.Type).ToArray());
        }
        else
        {
            if (!subs.Any() || args.Length == 1)
            {
                return parser.ParseArguments(args, _verbs.Select(x => x.Type).ToArray());
            }

            return parser.ParseArguments(args[1..], topLevel.Subs.Select(x => x.Type).ToArray());
        }
    }
}
