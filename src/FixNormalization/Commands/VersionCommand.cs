using FixNormalization.Arguments;
using Microsoft.VisualBasic;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;
using System.ComponentModel;
using System.Reflection;

namespace FixNormalization.Commands;

[GeneratedParser]
[Command]
[Description("Display version information.")]
[ParseOptions(IsPosix = true, CaseSensitive = false)]
internal sealed partial class VersionCommand : ICommand
{
    private readonly CommandLineParser _parser;

    public VersionCommand(CommandLineParser parser)
    {
        _parser = parser;
    }

    public int Run()
    {
        return (int)RootArguments.ShowVersion(_parser);
    }
}
