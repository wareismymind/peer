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
        var subMatch = subs.Any(x => x.Name == args.Skip(1).FirstOrDefault());
        var parser = new Parser(config =>
        {
            config.AutoHelp = true;
            config.AutoVersion = true;
            config.IgnoreUnknownArguments = false;
        });

        var state = new { rootVerb, subCount = subs.Length, subMatch };

        var parseResult = state switch
        {
            // If the first non-option token is not a recognized top-level command or the top-level command has no
            // subcommands then parse the complete command line.
            { rootVerb: null } or { subCount: 0 } => Parse(parser, args, _verbs),
            // If no subcommand name was specified, or the provided subcommand name does not refer to a subcommand of
            // the matched root verb, then skip the two verbs. This isn't perfect -- deal with it.
            { subMatch: false } => Parse(parser, args.Skip(2), subs),
            // Otherwise we've matched a top-level command and a subcommand so skip the top-level command token so
            // CommandLine.Parser will match the Options type corresponding to our subcommand.
            _ => Parse(parser, args[1..], subs),
        };

        var verb = _verbs.SelectMany(x => x.Subs.Append(x))
            .FirstOrDefault(x => x.Type == parseResult.TypeInfo.Current);

        return parseResult.MapResult(
            opts => new Result<Command, TextResult>(new Command(verb!, opts)),
            err => GetHelpText(verb, parseResult, err));
    }

    private static ParserResult<object> Parse(Parser parser, IEnumerable<string> args, IEnumerable<IVerb> verbs)
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
