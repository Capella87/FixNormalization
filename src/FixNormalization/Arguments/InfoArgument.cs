using Ookii.CommandLine;
using System.Text;
using Spectre.Console;
using System.ComponentModel;
using System.Reflection;

namespace FixNormalization.Arguments;

internal sealed partial class RootArguments
{
    [CommandLineArgument("info", IsShort = false)]
    [Description("Show information about this application.")]
    public static CancelMode Info(CommandLineParser parser)
    {
        AnsiConsole.Write(
            new FigletText("fnorm")
                .LeftJustified()
                .Color(Color.GreenYellow));
        Version(parser);
        AnsiConsole.MarkupLine(GetGitCommitInformation(parser.StringProvider, Assembly.GetExecutingAssembly()));
        AnsiConsole.MarkupLine($"\nCopyright (c) 2024-2026 Capella87.");
        AnsiConsole.MarkupLine("Distributed under MIT License.");
        AnsiConsole.MarkupLine("Repository: https://github.com/Capella87/FixNormalization");

        // Known Issue: CancelMode.Success will invoke the usage help
        return CancelMode.Abort;
    }
}
