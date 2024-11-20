using FixNormalization.Arguments;
using FixNormalization.Commands;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;

namespace FixNormalization;

public static class Program
{
    private static RootArguments Arguments { get; set; }

    public static async Task<int> Main(string[] args)
    {
        // For subcommands
        var appCommandOptions = new CommandOptions()
        {
            IsPosix = true,
            AutoVersionArgument = true,
            AutoVersionCommand = true,
            AutoHelpArgument = true,
            CommandNameTransform = NameTransform.DashCase,
            UsageWriter = new UsageWriter()
            {
                IncludeAliasInDescription = true,
                IncludeApplicationDescriptionBeforeCommandList = true,
            }
        };

        var appCommandManager = new AppCommandManager(appCommandOptions);

        // TODO: Invoke without Microsoft.Extensions.Hosting (Generic Host)
        // For root arguments
        var rootParseOptions = new ParseOptions()
        {
            IsPosix = true,
        };

        var parser = RootArguments.CreateParser(rootParseOptions);
        Arguments = parser.ParseWithErrorHandling();

        if (Arguments == null)
        {
            return 1;
        }


        var cts = new CancellationTokenSource();

        return await appCommandManager.RunCommandAsync(Arguments.Command,
            parser.ParseResult.RemainingArguments,
            cts.Token) ?? 1;
    }
}
