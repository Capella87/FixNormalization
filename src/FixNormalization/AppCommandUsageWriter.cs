using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;

namespace FixNormalization;

// Usage for subcommands in the root level
internal class AppCommandUsageWriter : UsageWriter
{
    public bool IncludeCommandUsageSyntax { get; set; }

    public AppCommandUsageWriter()
    {
    }

    protected override void WriteCommandListUsageSyntax()
    {
        if (IncludeCommandUsageSyntax)
        {
            base.WriteCommandListUsageSyntax();
        }
    }

    protected override void WriteCommandHelpInstruction(string name, string argumentNamePrefix, string argumentName)
    {
        base.WriteCommandHelpInstruction($"{name} [global-options]", argumentNamePrefix, argumentName);
    }
}
