using FixNormalization.Arguments;
using FixNormalization.Commands;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;

namespace FixNormalization;

public static class Program
{
    private static RootArguments? Arguments { get; set; }

    public static async Task<int> Main(string[] args)
    {
        var commandUsageWriter = new AppCommandUsageWriter()
        {
            IncludeAliasInDescription = true,
            IncludeApplicationDescriptionBeforeCommandList = true,
            IncludeApplicationDescription = true,
            IncludeCommandHelpInstruction = true,
        };

        // For subcommands
        var appCommandOptions = new CommandOptions()
        {
            IsPosix = true,
            AutoVersionCommand = false,
            AutoVersionArgument = false,
            AutoHelpArgument = true,
            CommandNameTransform = NameTransform.DashCase,
            UsageWriter = commandUsageWriter
        };

        var appCommandManager = new AppCommandManager(appCommandOptions);

        // TODO: Invoke without Microsoft.Extensions.Hosting (Generic Host) with CancelMode ? (Enable hosting with valid command.)
        // For root arguments
        var rootParseOptions = new ParseOptions()
        {
            IsPosix = true,
            UsageWriter = new AppArgumentUsageWriter(appCommandManager),
            AutoVersionArgument = true,
            ShowUsageOnError = UsageHelpRequest.Full,
        };

        var parser = RootArguments.CreateParser(rootParseOptions);
        Arguments = parser.ParseWithErrorHandling();

        if (Arguments == null)
        {
            return 1;
        }
        var cts = new CancellationTokenSource();

        commandUsageWriter.IncludeCommandUsageSyntax = true;
        try
        {
            return await appCommandManager.RunCommandAsync(Arguments.Command,
                parser.ParseResult.RemainingArguments,
                cts.Token) ?? 1;
        }
        catch (Exception ex)
        {
            // Anyway, we have to show the exception in detail..
            AnsiConsole.WriteException(ex, ExceptionFormats.Default);
            return 1;
        }
    }
}
