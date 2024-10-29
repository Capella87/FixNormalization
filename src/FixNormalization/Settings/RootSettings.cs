using Spectre.Console.Cli;

namespace FixNormalization.Settings;

/// <summary>
/// Represents the settings for main root command.
/// </summary>
public class RootSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to show version information of application.
    /// </summary>
    [CommandOption("-v|--version")]
    public bool? Version { get; set; }

    // Maybe hidden options...
}
