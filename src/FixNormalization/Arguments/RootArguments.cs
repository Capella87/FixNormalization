using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;
using System;
using System.ComponentModel;
using System.Reflection;

namespace FixNormalization.Arguments;

[GeneratedParser]
partial class RootArguments
{
    [CommandLineArgument(IsPositional = true, CancelParsing = CancelMode.Success)]
    [Description("Commands of fnorm.")]
    public required string Command { get; set; }

    /// <summary>
    /// Shows version information of application from assembly.
    /// </summary>
    /// <returns></returns>
    [CommandLineArgument]
    [Description("Display version information.")]
    public static CancelMode Version(CommandLineParser parser)
    {
        ShowRichVersionInformation(parser.StringProvider,
            Assembly.GetExecutingAssembly(),
            parser.ApplicationFriendlyName);

        // We must halt further parsing...
        return CancelMode.Abort;
    }

    private static void ShowRichVersionInformation(LocalizedStringProvider provider, Assembly assembly, string appName)
    {
        var version = GetAppVersion(assembly);
        var copyright = provider.ApplicationCopyright(assembly);
        // TODO: Get build date and git hash of application.

        AnsiConsole.MarkupLine($"[bold yellow]{appName}[/] {version.EscapeMarkup()}");
        Console.WriteLine();
    }

    private static string GetAppVersion(Assembly assembly)
    {
        var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var version = versionAttribute?.InformationalVersion ?? assembly.GetName()?.Version?.ToString() ?? string.Empty;

        return version;
    }
}
