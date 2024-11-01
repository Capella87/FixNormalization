using FixNormalization.Commands;
using FixNormalization.Settings;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.IO;
using System.CommandLine;

namespace FixNormalization;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Fix Unicode Normalization of each files");

        var builder = new CommandLineBuilder(rootCommand)
            .UseRichVersionOption()
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .CancelOnProcessTermination();
        return await builder.Build().InvokeAsync(args, new SpectreConsoleWrapper());
    }
}
