using System.CommandLine;
using System.CommandLine.IO;
using System.Runtime.CompilerServices;
using Spectre.Console;

namespace FixNormalization;

/// <summary>
/// Replaces System standard IO with Spectre.Console for System.CommandLine
/// </summary>
public class SpectreConsoleWrapper : IConsole
{
    public SpectreConsoleWrapper()
    {
        this.Out = StandardOutStreamWriter.Instance;
        this.Error = StandardErrorStreamWriter.Instance;
    }

    public IStandardStreamWriter Out { get; }

    public bool IsOutputRedirected => System.Console.IsOutputRedirected;

    public IStandardStreamWriter Error { get; }

    public bool IsErrorRedirected => System.Console.IsErrorRedirected;

    public bool IsInputRedirected => System.Console.IsInputRedirected;

    private struct StandardErrorStreamWriter : IStandardStreamWriter
    {
        public static readonly StandardErrorStreamWriter Instance = new();

        public void Write(string? value) => AnsiConsole.Markup(value);
    }

    private struct StandardOutStreamWriter : IStandardStreamWriter
    {
        public static readonly StandardOutStreamWriter Instance = new();

        public void Write(string? value) => AnsiConsole.Markup(value.EscapeMarkup());
    }


}
