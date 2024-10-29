using Spectre.Console.Cli;
using FixNormalization.Settings;
using Spectre.Console;

namespace FixNormalization.Commands;

public class FixCommand : Command<FixSettings>
{
    public override int Execute(CommandContext context, FixSettings settings)
    {
        foreach (var i in settings.Path)
        {
            AnsiConsole.WriteLine(i);
        }

        return 0;
    }
}
