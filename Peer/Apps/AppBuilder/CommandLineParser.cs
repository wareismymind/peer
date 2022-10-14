using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using wimm.Secundatives;
using Error = CommandLine.Error;

namespace Peer.Apps.AppBuilder;

public interface ICommandLineParser
{
    Result<Command, TextResult> Parse(string[] args);
}

public class CommandLineParser : ICommandLineParser
{
    private readonly List<IVerb> _verbs;

    public CommandLineParser(IEnumerable<IVerb> verbs)
    {
        _verbs = verbs.ToList();
    }

    //CN -- Should just swap this over to a different result type:
    // might be good to munge together the result type and the verb?
    // ArgInfo { IVerb, ParseResult<T> }? 
    // Actually it's really just a Result<IVerb, string>

    public Result<Command, TextResult> Parse(string[] args)
    {
        //I'm just dealing with one layer of nesting right now. I'm lazy
        var verbCount = args.Count(x => !x.StartsWith('-'));
        var rootVerb = _verbs.FirstOrDefault(x => x.Name == args.FirstOrDefault());
        var subs = rootVerb?.Subs.ToArray() ?? Array.Empty<IVerb>();
        var parser = new Parser(config =>
        {
            config.AutoHelp = true;
            config.AutoVersion = true;
            config.IgnoreUnknownArguments = false;
        });

        //Cases:
        // We don't find a verb that matches the first arg
        // We find a verb that matches the first arg
        //    There is more than one verb
        //    There is a single verb
        //        The verb has no handler

        var state = new { verbCount, rootVerb, subCount = subs.Length };
        // This is awful but no sleep makes for bad code
        // Now that I'm looking at this handling this in a loop might have been
        // better but I'm tired.
        var parseResult = state switch
        {
            //If someone puts in "schboop" they should get the default help
            { rootVerb: null } => Parse(parser, args, _verbs),
            //If someone puts in a verb with children/no handler they should get the help for the children of the verb
            { verbCount: 1, rootVerb.Handler: null } => Parse(parser, args[1..], subs),
            //If someone puts in multiple words but the root verb has no subverbs they should get the default help
            { verbCount: > 1, subCount: 0 } => Parse(parser, args, _verbs),
            // If someone puts in a valid set of verbs they should have the subverbs parsed
            { verbCount: > 1 } => Parse(parser, args[1..], subs),
            // If there is no specific case we should just handle it by default
            _ => Parse(parser, args, _verbs)
        };

        var verb = _verbs.SelectMany(x => x.Subs.Append(x))
            .FirstOrDefault(x => x.Type == parseResult.TypeInfo.Current);

        return parseResult.MapResult(
            opts => new Result<Command, TextResult> (new Command(verb!, opts)),
            err => GetHelpText(verb, parseResult, err));
    }

    private ParserResult<object> Parse(Parser parser, IEnumerable<string> args, IEnumerable<IVerb> verbs)
    {
        return parser.ParseArguments(args, verbs.Select(x => x.Type).ToArray());
    }
    
    private static TextResult GetHelpText(IVerb? verb, ParserResult<object> result, IEnumerable<Error> errors)
    {
        var help = verb?.CustomHelp?.GetHelpText(result) ?? HelpText.AutoBuild(result);

        foreach (var err in errors)
        {
            var type = err.Tag switch
            {
                ErrorType.HelpRequestedError => new Help(help),
                ErrorType.VersionRequestedError => new Version(help),
                _ => null as TextResult
            };

            if (type != null)
            {
                return type;
            }
        }

        return new UsageError(help);
    }
}
