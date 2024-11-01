using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.IO;
using System.Reflection;
using Spectre.Console;

namespace FixNormalization;

public static class CommandLineBuilderExtensions
{
    private static readonly Lazy<string> _appAssemblyVersion = new(() =>
    {
        var assembly = GetApplicationAssembly();
        var assemblyVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (assemblyVersionAttr is null)
        {
            return assembly.GetName().Version?.ToString() ?? " Unknown";
        }
        else
        {
            return assemblyVersionAttr.InformationalVersion;
        }
    });

    private static Assembly GetApplicationAssembly()
    {
        return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    }

    public static CommandLineBuilder UseRichVersionOption(this CommandLineBuilder builder)
    {
        var versionOption = new FixNormalization.Options.VersionOption(builder);

        builder.Command.AddOption(versionOption);
        builder.AddMiddleware(async (context, next) =>
        {
            if (context.ParseResult.FindResultFor(versionOption) is { })
            {
                if (context.ParseResult.Errors.Any(e => e.SymbolResult?.Symbol is FixNormalization.Options.VersionOption))
                {
                    // Use IInvocationResult to error report
                    // context.InvocationResult = new ParseErrorResult()
                }
                else
                {
                    // TODO: Add Git commit hash and release date
                    context.Console.Out.WriteLine($"[bold]FixNormalization[/] v{_appAssemblyVersion.Value.EscapeMarkup()}");
                }
            }
            else
            {
                await next(context);
            }
        }, (System.CommandLine.Invocation.MiddlewareOrder)(-1200));

        return builder;
    }
}
