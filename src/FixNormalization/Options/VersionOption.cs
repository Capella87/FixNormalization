using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Help;

namespace FixNormalization.Options;

public class VersionOption : Option<bool>
{
    private readonly CommandLineBuilder _builder;

    public VersionOption(CommandLineBuilder builder)
        : base(name: "--version", description: "Show version information")
    {
        _builder = builder;

        AddValidators();
    }

    // Source: System.CommandLine repository

    private void AddValidators()
    {
        AddValidator(result =>
        {
            if (result.Parent is { } parent &&
                parent.Children.Where(r => r.Symbol is not VersionOption)
                      .Any(IsNotImplicit))
            {
                result.ErrorMessage = result
                .LocalizationResources
                .VersionOptionCannotBeCombinedWithOtherArguments(result.Token?.Value ?? result.Symbol.Name);
            }
        });
    }

    private static bool IsNotImplicit(SymbolResult symbolResult)
    {
        return symbolResult switch
        {
            ArgumentResult argumentResult => !IsImplicitArgumentResult(argumentResult),
            OptionResult optionResult => !optionResult.IsImplicit,
            _ => true
        };
    }

    private static bool IsImplicitArgumentResult(ArgumentResult argumentResult)
    {
        return argumentResult.Argument.HasDefaultValue && argumentResult.Tokens.Count == 0;
    }

    public override string? Description { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is FixNormalization.Options.VersionOption;
    }

    public override int GetHashCode()
    {
        return typeof(FixNormalization.Options.VersionOption).GetHashCode();
    }
}
