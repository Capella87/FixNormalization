using Spectre.Console.Cli;
using FixNormalization.Commands;
using FixNormalization.Settings;

namespace FixNormalization;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.SetApplicationName("fnorm");

            // Add fix command and its options
            config.AddCommand<FixCommand>("fix")
                .WithDescription("Fix Unicode normalization problem in target's filename.")
                .WithExample(["fix", "path"])
                .WithExample(["fix", "path", "-r"])
                .WithExample(["fix", "path", "--recurse"]);
            // Add hidden option; this could be included by using diff tool.
        });

        return app.Run(args);
    }
}
