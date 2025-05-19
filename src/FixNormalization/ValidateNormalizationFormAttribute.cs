using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Ookii.CommandLine.Validation;
using System.Text;

namespace FixNormalization.Validation;

public class ValidateNormalizationFormAttribute : ArgumentValidationWithHelpAttribute
{
    // TODO: Implement dedicated UsageHelp for NormalizationForm for aliases
    // Temporary implementation: Just shows exact enum values
    protected override string GetUsageHelpCore(CommandLineArgument argument) => argument.Parser.StringProvider.ValidateEnumValueUsageHelp(typeof(NormalizationForm));

    public override string GetErrorMessage(CommandLineArgument argument, object? value)
    => argument.Parser.StringProvider.ValidationFailed(argument.ArgumentName);
}
