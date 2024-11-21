using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using System.ComponentModel;

namespace FixNormalization.Arguments;

[GeneratedParser]
partial class RootArguments
{
    [CommandLineArgument(IsPositional = true, CancelParsing = CancelMode.Success)]
    [Description("Commands of fnorm.")]
    public required string Command { get; set; }
}
