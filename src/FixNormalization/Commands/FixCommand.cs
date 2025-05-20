using Ookii.CommandLine.Commands;
using Spectre.Console;
using System.ComponentModel;
using Ookii.CommandLine;
using System.Text;
using System.Security;
using Ookii.CommandLine.Conversion;
using System.IO.Abstractions;

using Ookii.CommandLine.Validation;

namespace FixNormalization.Commands;

[GeneratedParser]
[Command]
[Description("Unify the Unicode normalization of files for compatibility.")]
[ParseOptions(IsPosix = true,
    CaseSensitive = true,
    ArgumentNameTransform = NameTransform.DashCase,
    ValueDescriptionTransform = NameTransform.DashCase)]
public partial class FixCommand : AsyncCommandBase
{
    [CommandLineArgument("target", IsPositional = false, Position = 0)]
    [Description("Paths of files or directories' which contain files to be normalized.")]
    [ValueDescription("path")]
    public required string[]? Target { get; set; }

    [CommandLineArgument("form", IsRequired = false, DefaultValue = NormalizationForm.FormC)]
    [Description("Normalization form to be used. You can choose NFC (The most common types in the majority of environments) and NFD (Used in macOS or Darwin)")]
    [ValueDescription("form")]
    [ArgumentConverter(typeof(NormalizationFormConverter))]
    [ValidateEnumValue(AllowNonDefinedValues = TriState.True,
        IncludeInUsageHelp = true,
        IncludeValuesInErrorMessage = true)]
    public NormalizationForm NForm { get; set; }

    [CommandLineArgument(IsShort = true)]
    [Description("Display more debug information.")]
    public bool Verbose { get; set; }

    [CommandLineArgument(IsShort = true)]
    [Description("Display only notable warnings and errors.")]
    public bool Quiet { get; set; }

    [CommandLineArgument(IsShort = true)]
    [Description("Enable recursive option. Process all files also in subdirectories.")]
    public bool Recursive { get; set; }

    private int _successCount = 0;

    private IFileSystem? _fileSystem;

    public FixCommand() : base()
    {
        _fileSystem = null;
    }

    // TODO: Excluded item criteria (wildcard, file type... etc)

    public async Task<int> RunAsync(IFileSystem? fs, CancellationToken token)
    {
        _fileSystem = fs;
        return await RunAsync(token);
    }

    public override async Task<int> RunAsync(CancellationToken token)
    {
        _fileSystem ??= new FileSystem();

        var targetedFiles = new List<string>()!;
        foreach (var entity in Target)
        {
            string? e = GetAbsolutePath(entity);
            if (e is null) continue;

            // Check the existence of the path
            if (!_fileSystem!.Path.Exists(e))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Path '{e.EscapeMarkup()}' does not exist.");
                continue;
            }

            switch (CheckPathObjectType(e))
            {
                case PathObjectTypes.Directory:

                    // Check all files in the directory
                    await DetectFilesInDirectory(e, targetedFiles, isRecursive: Recursive
                        , form: NForm, null);
                    break;

                case PathObjectTypes.NormalFile:

                    if (!e.IsNormalized(NForm))
                    {
                        // TODO: Show the file with not normalized filename to NForm in verbose mode
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
            AnsiConsole.MarkupLine($"[red]Error: there's no such valid file to normalize.[/]");
            return 22;
        }

        var failed = new List<string>();
        try
        {
            await NormalizeFiles(targetedFiles, failed, token);
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
            // TODO: Redirect the output to file when user enabled log option.

            // Temporary implementation
            AnsiConsole.WriteLine("Done.");
            if (!Quiet)
            {
                Console.Beep();
            }
            AnsiConsole.MarkupLine($"[green][bold]Success[/]: {_successCount}[/], [red]Failed: {failed.Count}[/]");
        }

        return 0;
    }

    private async Task NormalizeFiles(List<string> files, List<string> failed, CancellationToken ct)
    {
        foreach (var file in files)
        {
            var basePath = _fileSystem!.Path.GetDirectoryName(file);
            var normalizedFilename = _fileSystem.Path.GetFileName(file).Normalize(NForm);
            try
            {
                var normalizedPath = Path.Join(basePath, normalizedFilename);
                await Task.Run(() => _fileSystem!.File.Move(file, normalizedPath), ct);
                AnsiConsole.MarkupLine($"[green]Success:[/] File '{file.EscapeMarkup()}' has been converted to '{normalizedFilename.EscapeMarkup()}' following {Enum.GetName(typeof(NormalizationForm), NForm)}.");
                _successCount++;
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine($"[orange][bold]Operation canceled[/] while renaming '{file.EscapeMarkup()}'[/]");
                failed.Add(file);
                throw;
            }
            catch (PathTooLongException)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] File path '{file.EscapeMarkup()}' is too long to process on this system.");
                failed.Add(file);
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Access denied to '{file.EscapeMarkup()}'.");
                failed.Add(file);
            }
            catch (IOException)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] An I/O error occurred while processing '{file.EscapeMarkup()}'.");
                failed.Add(file);
            }
            catch (Exception)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] There was a problem while processing '{file.EscapeMarkup()}'.");
                failed.Add(file);
                throw;
            }

            ct.ThrowIfCancellationRequested();
        }
    }

    private async Task<int> DetectFilesInDirectory(string path, List<string> files, bool isRecursive, string searchPatterns, NormalizationForm form, int? level = null)
    {
        // TODO: Separate detection work for async and multi-threading...
        // TODO: Search pattern support (Mandated thing to support recursion)
        // TODO: Show targeted files of directory in verbose mode.
        // TODO: Show debug information or write them to log file in debug mode...
        List<string>? dirs = new List<string>() { path };
        IEnumerable<string>? detected = null;

        try
        {
            if (isRecursive)
            {
                AnsiConsole.MarkupLine("[yellow][bold]Info[/]: Recursive mode is enabled. Now files in subdirectories will also be processed.[/]");
                dirs.AddRange(GetSubDirectories(path, ""));
            }

            foreach (var d in dirs)
            {
                // All subdirectories are included in the list to be processed.
                detected = _fileSystem!.Directory.EnumerateFiles(d, searchPatterns, SearchOption.TopDirectoryOnly)
                    .Where(f => !_fileSystem.Path.GetFileName(f).IsNormalized(form));
                if (detected is not null)
                {
                    files.AddRange(detected);
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            AnsiConsole.MarkupLine($"[red]Error[/]: Directory '{path.EscapeMarkup()}'[/] is not found.");
            return 2;
        }
        catch (SecurityException)
        {
            AnsiConsole.MarkupLine($"[red]Error[/]: Directory '{path.EscapeMarkup()}' has security problem. (e.g. privileges)");
            return 13;
        }
        catch (PathTooLongException)
        {
            AnsiConsole.MarkupLine($"[red]Error[/]: Path '{path.EscapeMarkup()}' exceeds the maximum length of path.");
            return 36;
        }
        catch (ArgumentException)
        {
            // TODO: Include stack trace information for debug
            AnsiConsole.MarkupLine($"[red]Error[/]: There was a problem to process path '{path.EscapeMarkup()}'.");
            return 22;
        }
        catch (IOException)
        {
            // TODO: Include stack trace information for debug
            AnsiConsole.MarkupLine($"[red]Error[/]: An I/O problem occurred while processing path '{path.EscapeMarkup()}'.");
            return 5;
        }
        return await Task<int>.FromResult(detected!.Count());
    }

    [Obsolete("Use DetectFilesInDirectory(string, List<string>, bool, string, NormalizationForm, int?) instead.", error: false)]
    private async Task<int> DetectFilesInDirectory(string path, List<string> files,
        bool isRecursive,
        NormalizationForm form,
        int? level = null)
    {
        return await DetectFilesInDirectory(path, files, isRecursive, "*.*", form, level);
    }

    private IEnumerable<string> GetSubDirectories(string basePath, string searchPatterns) => _fileSystem!.Directory.EnumerateDirectories(basePath, searchPatterns, SearchOption.AllDirectories);

    private PathObjectTypes CheckPathObjectType(string path)
    {
        if (_fileSystem!.Directory.Exists(path))
        {
            return PathObjectTypes.Directory;
        }

        if (_fileSystem.File.Exists(path))
        {
            return PathObjectTypes.NormalFile;
        }

        return PathObjectTypes.Unknown;
    }

    private string? GetAbsolutePath(string? path)
    {
        string? rt = null;
        try
        {
            rt = _fileSystem!.Path.GetFullPath(path);
        }
        catch (PathTooLongException)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Path {path.EscapeMarkup()} exceeds the maximum length of path.");
            rt = null;
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Path {path.EscapeMarkup()} is an invalid path.");
            rt = null;
        }

        return rt;
    }
}
