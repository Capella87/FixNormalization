using FixNormalization.Arguments;
using FixNormalization.Commands;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;
using System.Text;

namespace FixNormalization;

public static class Program
{
    private static RootArguments? Arguments { get; set; }

    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var commandUsageWriter = new AppCommandUsageWriter()
        {
            IncludeAliasInDescription = true,
            IncludeApplicationDescriptionBeforeCommandList = true,
            IncludeApplicationDescription = true,
            IncludeCommandHelpInstruction = TriState.True,
        };

        // For subcommands
        var appCommandOptions = new CommandOptions()
        {
            IsPosix = true,
            AutoVersionCommand = false,
            AutoVersionArgument = false,
            AutoHelpArgument = true,
            CommandNameTransform = NameTransform.DashCase,
            UsageWriter = commandUsageWriter,
            ShowUsageOnError = UsageHelpRequest.Full,
        };

        var appCommandManager = new AppCommandManager(appCommandOptions);

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
