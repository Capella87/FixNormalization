using Ookii.CommandLine;
using Ookii.CommandLine.Conversion;
using Ookii.CommandLine.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

using FixNormalization.Validation;

namespace FixNormalization;

public sealed class NormalizationFormConverter : ArgumentConverter
{
    public override object? Convert(ReadOnlySpan<char> value, CultureInfo culture, CommandLineArgument argument)
    {
        return Convert(value.ToString(), culture, argument);
    }

    public override object? Convert(string value, CultureInfo culture, CommandLineArgument argument)
    {
        var attribute = argument.Validators.OfType<ValidateNormalizationFormAttribute>()!.FirstOrDefault();

        try
        {
            return ParseNormalizationForm(value);
        }
        catch (ArgumentNullException ex)
        {
            throw new CommandLineArgumentException(GetExceptionMessage(value, argument, attribute),
                argument.ArgumentName,
                CommandLineArgumentErrorCategory.ArgumentValueConversion,
                ex);
        }
        catch (ArgumentException ex)
        {
            throw new CommandLineArgumentException(GetExceptionMessage(value, argument, attribute),
                argument.ArgumentName,
                CommandLineArgumentErrorCategory.ArgumentValueConversion,
                ex);
        }
    }

    internal static object? ParseNormalizationForm(string value)
    {
        ReadOnlySpan<char> lowered = value.ToLower().AsSpan();
        return ConvertToNormalizationForm(lowered);
    }

    private static NormalizationForm ConvertToNormalizationForm(ReadOnlySpan<char> target) => target switch
    {
        "nfd" or "formd" or "macos" or "darwin" => NormalizationForm.FormD,
        "nfc" or "formc" or "windows" or "linux" => NormalizationForm.FormC,
        _ => throw new ArgumentException($"Value {target.ToString()} is invalid.")
    };

    private string GetExceptionMessage(string value, CommandLineArgument argument, ValidateNormalizationFormAttribute? attr)
    {
        return attr!.GetErrorMessage(argument, value) ?? argument.Parser.StringProvider.ValidateEnumValueFailed(argument.ArgumentName, typeof(NormalizationForm), value, true);
    }
}
