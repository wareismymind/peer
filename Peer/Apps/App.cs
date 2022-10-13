using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps.AppBuilder;
using Peer.Domain.Configuration;
using wimm.Secundatives;

namespace Peer.Apps;

public class App
{
    private readonly ICommandLineParser _parser;
    private readonly List<IVerb> _verbs;
    private readonly IServiceSetupHandler? _setupHandler;

    //CN - This is a temp thing - Really we just should fail out if config fails probably.
    // Alternatively since there's a few different cases where it doesn't matter maybe we don't
    // care about if it succeeds or fails at this point - or break up the
    // different handlers.. Since we don't care about loading config till later in 
    // the process
    // I think really what we want is: 
    // AppBuilderTime - Only the stuff required to Create handlers/junk
    // PreParse - Only the common setup stuff for each handler (Stuff common to everybody - probably don't *need* this)
    // VerbConfig - All of the stuff required to run specific verbs - initialized right before running it (probably Verb.CustomConfig or similar)
    private static readonly Dictionary<ConfigError, string> _configErrorMap = new()
    {
        [ConfigError.InvalidProviderValues] = "One or more providers have invalid configuration",
        [ConfigError.NoProvidersConfigured] = "No providers are configured! Run 'peer config' to get started",
        [ConfigError.ProviderNotMatched] = "Provider was not recognized, make sure you're using one of supported providers!"
    };
    
    public App(ICommandLineParser parser, IEnumerable<IVerb> verbs, IServiceSetupHandler? setupHandler = null)
    {
        _parser = parser;
        _verbs = verbs.ToList();
        _setupHandler = setupHandler;

        if (_verbs.Count == 0)
        {
            throw new ArgumentException("There must be at least one configured verb");
        }
    }

    public async Task<int> RunAsync(string[] args, CancellationToken token = default)
    {
        var result = _parser.Parse(args);
        var services = new ServiceCollection();
        _setupHandler?.SetupServices(services);
        if (result.IsValue)
        {
            try
            {
                var value = result.Value;
                await value.Verb.Handler!.HandleAsync(value.Options, services, token);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
        else
        {
            Console.WriteLine(result.Error.Text);
            return result.Error is UsageError ? 1 : 0;
        }
    }

    private static Task WriteHelpText(IVerb? verb, ParserResult<object> result)
    {
        var help = verb?.CustomHelp?.GetHelpText(result) ?? HelpText.AutoBuild(result);
        Console.Write(help);
        return Task.CompletedTask;
    }
}
