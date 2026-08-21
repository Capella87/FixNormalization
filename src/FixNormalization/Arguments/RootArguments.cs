using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Spectre.Console;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FixNormalization.Arguments;

[GeneratedParser]
[Description("Fix Unicode normalization of filenames.")]
internal sealed partial class RootArguments
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
        return ShowVersion(parser);
    }

    public static CancelMode ShowVersion(CommandLineParser parser)
    {
        var assembly = Assembly.GetExecutingAssembly();

        if (assembly is null)
        {
            AnsiConsole.MarkupLine($"[yellow bold]{parser.ApplicationFriendlyName}[/] Unknown version");
            return CancelMode.Abort;
        }

        ShowRichVersionInformation(parser.StringProvider,
            assembly,
            parser.ApplicationFriendlyName);

        // We must halt further parsing...
        return CancelMode.Abort;
    }

    public static void ShowRichVersionInformation(LocalizedStringProvider provider, Assembly assembly, string appName)
    {
        var version = GetAppVersion(assembly);
        var copyright = provider.ApplicationCopyright(assembly);
        // TODO: Get build date and git hash of application.

        AnsiConsole.MarkupLine($"[bold yellow]{appName}[/] {version.EscapeMarkup()}");
    }

    public static string GetGitCommitInformation(LocalizedStringProvider stringProvider, Assembly assembly)
    {
        var stringBuilder = new StringBuilder();

        var attrs = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();

        var sha = attrs
                            .FirstOrDefault(a => a.Key == "GitCommitSha")?
                            .Value ?? "Unknown";
        var branch = attrs.FirstOrDefault(a => a.Key == "GitBranch")?.Value ?? "Unknown";
        var date = attrs
                            .FirstOrDefault(a => a.Key == "GitCommitDate")?
                            .Value ?? "Unknown";
        if (sha != "Unknown" && date != "Unknown")
        {
            return stringBuilder.Append($"{branch}-{sha} ({date})")
                .ToString();
        }

        return string.Empty;
    }

    private static string GetAppVersion(Assembly assembly)
    {
        var versionAttribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        var version = versionAttribute?.FirstOrDefault(a => a.Key == "SemVer")?.Value ?? assembly.GetName()?.Version?.ToString() ?? string.Empty;

        return version;
    }
}
