using Xunit;
using Xunit.Abstractions;
using NSubstitute;
using System.Text;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using FixNormalization.Commands;

namespace FixNormalization.Tests;

public class FormCNormalizationTests : IClassFixture<FileSystemFixture>
{
    FileSystemFixture fixture;

    private readonly ITestOutputHelper _output;

    public FormCNormalizationTests(FileSystemFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        _output = output;
    }

    [InlineData("./")]
    [InlineData("./FirstSubDir/옛한글")]
    [InlineData("./FirstSubDir")]
    [InlineData("./Western/")]
    [Theory]
    public async Task FormCFileSystem_Directory_Should_Be_Normalized_To_FormD_NonRecursive(string directoryPath)
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormC);

        var targetParser = new CommandLineParser<FixCommand>();

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);
        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([directoryPath, "--form", "nfd", "-q"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        foreach (var f in fixture.FileSystem!.Directory.GetFiles(directoryPath))
        {
            var filename = fixture.FileSystem.Path.GetFileName(f);

            Assert.True(filename.IsNormalized(NormalizationForm.FormD));
        }

        // Assert for files which should not be normalized in subdirectories
        Assert.All(this.fixture.FileSystem.Directory.GetDirectories(directoryPath),
            (f) => fixture.FileSystem.Directory.GetFiles(f, "", SearchOption.AllDirectories)
            .All(filePath => !(Path.GetFileName(filePath)
            .IsNormalized(NormalizationForm.FormD))));
    }

    [Fact]
    public async Task FormCFileSystem_Directories_Should_Be_Normalized_To_FormD_Recursive()
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormC);

        var targetParser = new CommandLineParser<FixCommand>();
        string rootPath = fixture.FileSystem!.Path.GetPathRoot(fixture.FileSystem.Directory.GetCurrentDirectory())!;

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);

        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([rootPath, "--form", "nfd", "-q", "-r"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        foreach (var f in fixture.FileSystem!.Directory.GetFiles(rootPath, "", SearchOption.AllDirectories))
        {
            var filename = fixture.FileSystem.Path.GetFileName(f);

            Assert.True(filename.IsNormalized(NormalizationForm.FormD));
        }
    }

    [InlineData("./어린양.txt")]
    [InlineData("./Western/Áçčèñţşůşîñģdïäçřïţïçš.txt")]
    [Theory]
    public async Task FormCFileSystem_File_Should_Be_Normalized_To_FormD(string filePath)
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormC);

        var targetParser = new CommandLineParser<FixCommand>();

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);

        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([filePath, "--form", "nfd", "-q"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        var filename = fixture.FileSystem!.Path.GetFileName(filePath).Normalize(NormalizationForm.FormD);
        Assert.True(filename.IsNormalized(NormalizationForm.FormD));
    }
}
