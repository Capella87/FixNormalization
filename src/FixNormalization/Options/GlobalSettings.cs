using Spectre.Console.Cli;

namespace FixNormalization.Settings;

/// <summary>
/// Represents the settings for all commands.
/// </summary>
public class GlobalSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to show verbose output.
    /// </summary>
    [CommandOption("--verbose")]
    public bool? Verbosity { get; set; }

    /// <summary>
    /// Gets or sets a user-desired log file path. If user only specifies like a flag, then it will be set to default path.
    /// </summary>
    [CommandOption("--log")]
    public string? LogPath { get; set; }
}
