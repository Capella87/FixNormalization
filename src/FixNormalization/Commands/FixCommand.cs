using Spectre.Console.Cli;
using FixNormalization.Settings;
using Spectre.Console;
using System.ComponentModel;
using System.Diagnostics;

namespace FixNormalization.Commands;

public class FixCommand : Command<FixSettings>
{
    public override int Execute(CommandContext context, FixSettings settings)
    {
        var targetedFiles = new List<string>();
        var directories = new List<string>();

        // Check paths
        foreach (var i in settings.Path)
        {
            // Check whether the path exists
            if (!System.IO.Directory.Exists(i))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Path '{i}' does not exist.");
                return 1;
            }

            if (System.IO.File.Exists(i))
            {
                if (!i.IsNormalized(System.Text.NormalizationForm.FormC))
                {
                    targetedFiles.Add(i);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Info:[/] File '{i}' is not normalized file with NFD.");
                }
            }
            else
            {
                directories.Add(i);
            }
        }

        // Detect and add files with wrong Unicode normalization in given paths to list to be processed.
        // This can be utilized with threads..
        // TODO: Thread Utilization
        // TODO: Show a progress bar and status while processing files.
        foreach (var d in directories)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(d))
            {
                if (file.IsNormalized(System.Text.NormalizationForm.FormD))
                {
                    AnsiConsole.MarkupLine($"[yellow]Info:[/] File '{file.EscapeMarkup()}' is normalized file with NFD.");
                    targetedFiles.Add(file);
                }
            }
        }

        int successed = 0;
        var failed = new List<string>();
        foreach (var file in targetedFiles)
        {
            var normalizedFilename = file.Normalize(System.Text.NormalizationForm.FormC);
            try
            {
                System.IO.File.Move(file, normalizedFilename);
                AnsiConsole.MarkupLine($"[green]Success:[/] File '{file.EscapeMarkup()}' has been normalized to Form C.");
                successed++;
            }
            catch (UnauthorizedAccessException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Access denied to '{file.EscapeMarkup()}'.");
                failed.Add(normalizedFilename);
            }
            catch (System.IO.IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] An I/O error occurred while processing '{file.EscapeMarkup()}'.");
                failed.Add(normalizedFilename);
            }
        }

        // Show results
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.Markup($"[green]Success: {successed}[/] ");
        AnsiConsole.MarkupLine($"[red]Failed: {failed.Count}[/]");

        return 0;
    }
}
