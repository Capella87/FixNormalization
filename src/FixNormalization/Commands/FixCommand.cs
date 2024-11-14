using Ookii.CommandLine.Commands;
using Spectre.Console;
using System.ComponentModel;
using Ookii.CommandLine;
using System.Text;
using System.Security;

namespace FixNormalization.Commands;

[GeneratedParser]
[Command]
[Description("Unify the Unicode normalization of files for compatibility.")]
public partial class FixCommand : AsyncCommandBase
{
    [CommandLineArgument("target", IsPositional = true, IsRequired = true, Position = 0)]
    [Description("Files or directories which contain files to be normalized.")]
    [ValueDescription("target")]
    public required string[] Target { get; set; }

    [CommandLineArgument("form", IsRequired = false, DefaultValue = NormalizationForm.FormC)]
    [Description("Normalization form to be used. You can choose NFC (The most common types in majority of environments) and NFD (Used in macOS or Darwin)")]
    [ValueDescription("form")]
    public NormalizationForm NForm { get; set; }

    private int _successCount = 0;

    // TODO: excluded item criteria (wildcard, file type... etc)

    public override async Task<int> RunAsync()
    {
        var targetedFiles = new List<string>()!;
        foreach (var entity in Target)
        {
            string? e = GetAbsolutePath(entity);
            if (e is null) continue;

            // Check the existence of the path
            if (!Path.Exists(e))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Path '{e.EscapeMarkup()}' does not exist.");
                continue;
            }


            switch (CheckPathObjectType(e))
            {
                case PathObjectTypes.Directory:

                    // Check all files in the directory
                    // TODO: Support recursive detection
                    await DetectFilesInDirectory(e, targetedFiles, isRecursive: false
                        , form: NForm, null);
                    break;

                case PathObjectTypes.NormalFile:

                    if (!e.IsNormalized(NormalizationForm.FormC))
                    {
                        targetedFiles.Add(e);
                    }
                    else
                    {
                        // TODO: Show non-targeted files information in verbose mode.
                        // TODO: Implement verbose output method
                    }
                    break;
                default:
                    break;
            }
        }

        if (targetedFiles.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]Error: there's no such valid file to process.[/]");
            return 22;
        }

        var failed = new List<string>();
        try
        {
            await NormalizeFiles(targetedFiles, failed, this.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            return 125;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths
                | ExceptionFormats.ShortenTypes
                | ExceptionFormats.ShortenMethods
                | ExceptionFormats.ShowLinks);
            return 1;
        }
        finally
        {
            // Show statistics
            // TODO: Silent option to skip showing results
            // TODO: Redirect it to file when user enabled log option.

        }

        return 0;
    }

    private async Task NormalizeFiles(List<string> files, List<string> failed, CancellationToken ct)
    {
        foreach (var file in files)
        {
            var normalizedFilename = file.Normalize(System.Text.NormalizationForm.FormC);
            try
            {
               await Task.Run(() => File.Move(file, normalizedFilename), ct);
                AnsiConsole.MarkupLine($"[green]Success:[/] File '{file.EscapeMarkup()}' has been normalized to Form C.");
                _successCount++;
            }
            catch (OperationCanceledException ex)
            {
                AnsiConsole.MarkupLine($"[orange][bold]Operation canceled[/] while renaming '{file.EscapeMarkup()}'[/]");
                failed.Add(file);
                throw;
            }
            catch (PathTooLongException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] File path '{file.EscapeMarkup()}' is too long to process on this system.");
                failed.Add(file);
            }
            catch (UnauthorizedAccessException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Access denied to '{file.EscapeMarkup()}'.");
                failed.Add(file);
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] An I/O error occurred while processing '{file.EscapeMarkup()}'.");
                failed.Add(file);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] There was a problem while processing '{file.EscapeMarkup()}'.");
                failed.Add(file);
                throw;
            }

            ct.ThrowIfCancellationRequested();
        }
    }

    private async Task<int> DetectFilesInDirectory(string path, List<string> files,
        bool isRecursive,
        NormalizationForm form,
        int? level = null)
    {
        // TODO: Search files with threads (Count and listing require locking..)
        // TODO: Separate detection work for async and multi-threading...
        // TODO: Search pattern support (Mandated thing to support recursion)
        // TODO: Show targeted files of directory in verbose mode.
        // TODO: Show debug information or write them to log file in debug mode...
        IEnumerable<string>? detected = null;
        try
        {
            detected = Directory.EnumerateFiles(path)
                .Where(f => !f.IsNormalized(form));
        }
        catch (DirectoryNotFoundException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: Directory {path.EscapeMarkup()}[/] is not found.");
            return 2;
        }
        catch (SecurityException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error[/]: Directory {path.EscapeMarkup()} has security problem. (e.g. privileges)");
            return 13;
        }
        catch (PathTooLongException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error[/]: Path {path.EscapeMarkup()} exceeds the maximum length of path.");
            return 36;
        }
        catch (ArgumentException ex)
        {
            // TODO: Include stack trace information for debug
            AnsiConsole.MarkupLine($"[red]Error[/]: There was a problem to process path {path.EscapeMarkup()}");
            return 22;
        }
        catch (IOException ex)
        {
            // TODO: Include stack trace information for debug
            AnsiConsole.MarkupLine($"[red]Error[/]: An I/O problem occurred while processing path {path.EscapeMarkup()}");
            return 5;
        }

        if (detected is not null)
        {
            files.AddRange(detected);
        }

        return await Task<int>.FromResult(detected!.Count());
    }

    private static PathObjectTypes CheckPathObjectType(string path)
    {
        if (Directory.Exists(path))
        {
            return PathObjectTypes.Directory;
        }

        if (File.Exists(path))
        {
            return PathObjectTypes.NormalFile;
        }

        return PathObjectTypes.Unknown;
    }

    private static string? GetAbsolutePath(string? path)
    {
        string? rt = null;
        try
        {
            rt = Path.GetFullPath(path);
        }
        catch (PathTooLongException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Path {path.EscapeMarkup()} exceeds the maximum length of path.");
            rt = null;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Path {path.EscapeMarkup()} is an invalid path.");
            rt = null;
        }

        return rt;
    }
}
