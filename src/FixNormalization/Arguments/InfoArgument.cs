using Ookii.CommandLine;
using System.Text;
using Spectre.Console;
using System.ComponentModel;

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
        Console.WriteLine('\n');
        AnsiConsole.MarkupLine($"Copyright (c) 2024-2025 Capella87.");
        AnsiConsole.MarkupLine("Distributed under MIT License.");
        AnsiConsole.MarkupLine("Repository: https://github.com/Capella87/FixNormalization");

        return CancelMode.Abort;
    }
}
