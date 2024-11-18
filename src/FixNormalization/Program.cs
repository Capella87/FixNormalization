using FixNormalization.Commands;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;

namespace FixNormalization;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // var rootCommand = new RootCommand("Fix Unicode Normalization of each files");

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

        var appCommandManager = new CommandManager(appCommandOptions);

        var cts = new CancellationTokenSource();

        return await appCommandManager.RunCommandAsync(cts.Token) ?? 1;
    }
}
