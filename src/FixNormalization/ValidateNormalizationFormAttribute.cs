using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Ookii.CommandLine.Validation;
using System.Text;

namespace FixNormalization.Validation;

public class ValidateNormalizationFormAttribute : ArgumentValidationWithHelpAttribute
{
    public bool AllowNonDefinedValues { get; set; }

    public bool IncludeInUsageHelp { get; set; }

    public bool IncludeValuesInErrorMessage { get; set; }

    public override bool IsValid(CommandLineArgument argument, object? value)
    {
        return AllowNonDefinedValues || value is null || argument.ElementType.IsEnumDefined(value);
    }

    // TODO: Implement dedicated UsageHelp for NormalizationForm for aliases
    // Temporary implementation: Just shows exact enum values
    protected override string GetUsageHelpCore(CommandLineArgument argument) => argument.Parser.StringProvider.ValidateEnumValueUsageHelp(typeof(NormalizationForm));

    public virtual string GetErrorMessage(CommandLineArgument argument, object? value)
    => argument.Parser.StringProvider.ValidationFailed(argument.ArgumentName);
}
