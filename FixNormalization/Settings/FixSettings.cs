using Spectre.Console.Cli;

namespace FixNormalization.Settings;

/// <summary>
/// Represents the settings for fix command.
/// </summary>
public class FixSettings : GlobalSettings
{
    [CommandArgument(0, "[path]")]
    public required string[] Path { get; set; }

    [CommandOption("-r|--recurse")]
    public bool? Recursion { get; set; }
}
