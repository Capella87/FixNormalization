using Ookii.CommandLine;
using Ookii.CommandLine.Commands;

namespace FixNormalization;

internal class AppArgumentUsageWriter : UsageWriter
{
    private readonly CommandManager _commandManager;

    public AppArgumentUsageWriter(CommandManager commandManager)
    {
        _commandManager = commandManager;
    }

    protected override void WriteUsageSyntaxSuffix()
    {
        Writer.Write(" [command-options]");
    }

    protected override void WriteArgumentDescriptions()
    {
        base.WriteArgumentDescriptions();
        _commandManager.WriteUsage();
    }

    protected override void WriteSwitchValueDescription(string valueDescription)
    {
        // Intentionally left blank... No reason to write switch value description.
    }
}
