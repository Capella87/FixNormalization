using Ookii.CommandLine;
using Ookii.CommandLine.Conversion;
using Ookii.CommandLine.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace FixNormalization;

public sealed class NormalizationFormConverter : ArgumentConverter
{
    public override object? Convert(ReadOnlySpan<char> value, CultureInfo culture, CommandLineArgument argument)
    {
        return Convert(value.ToString(), culture, argument);
    }

    public override object? Convert(string value, CultureInfo culture, CommandLineArgument argument)
    {
        try
        {
            return ParseNormalizationForm(value);
        }
        catch (ArgumentNullException ex)
        {
            throw new CommandLineArgumentException();
        }
        catch (ArgumentException ex)
        {
            throw new NotImplementedException();
        }
    }

    internal static object? ParseNormalizationForm(string value)
    {

        ReadOnlySpan<char> lowered = value.ToLower().AsSpan();

        return (ReadOnlySpan<char> lowered) => lowered switch
        {
            "nfd" or "formd" or "macos" or "darwin" => NormalizationForm.FormD,
            "nfc" or "formc" or "windows" or "linux" => NormalizationForm.FormC,
            _ => throw new ArgumentException($"Value {value.ToString()} is invalid.")
        };
    }

    private GetExceptionMessage(string value, Exception? inner, CommandLineArgument argument, ValidationEnum)
}
