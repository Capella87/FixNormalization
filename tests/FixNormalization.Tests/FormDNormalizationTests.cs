using Xunit;
using Xunit.Abstractions;
using NSubstitute;
using System.Text;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using FixNormalization.Commands;

namespace FixNormalization.Tests;

public class FormDNormalizationTests : IClassFixture<FileSystemFixture>
{
    FileSystemFixture fixture;

    private readonly ITestOutputHelper _output;

    public FormDNormalizationTests(FileSystemFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        _output = output;
    }

    [InlineData("./")]
    [InlineData("./FirstSubDir/옛한글")]
    [InlineData("./FirstSubDir")]
    [InlineData("./Western/")]
    [Theory]
    public async Task FormDFileSystem_Directory_Should_Be_Normalized_To_FormC_NonRecursive(string directoryPath)
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormD);

        var targetParser = new CommandLineParser<FixCommand>();

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);

        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([directoryPath, "-q"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        foreach (var f in fixture.FileSystem!.Directory.GetFiles(directoryPath))
        {
            var filename = fixture.FileSystem.Path.GetFileName(f);

            Assert.True(filename.IsNormalized(NormalizationForm.FormC));
        }

        // Assert for files which should not be normalized in subdirectories
        Assert.All(this.fixture.FileSystem.Directory.GetDirectories(directoryPath),
            (f) => fixture.FileSystem.Directory.GetFiles(f, "", SearchOption.AllDirectories)
            .All(filePath => !(Path.GetFileName(filePath)
            .IsNormalized(NormalizationForm.FormC))));
    }

    [Fact]
    public async Task FormDFileSystem_Directories_Should_Be_Normalized_To_FormC_Recursive()
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormD);

        var targetParser = new CommandLineParser<FixCommand>();
        string rootPath = fixture.FileSystem!.Path.GetPathRoot(fixture.FileSystem.Directory.GetCurrentDirectory())!;

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);

        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([rootPath, "-q", "-r"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        foreach (var f in fixture.FileSystem!.Directory.GetFiles(rootPath, "", SearchOption.AllDirectories))
        {
            var filename = fixture.FileSystem.Path.GetFileName(f);

            Assert.True(filename.IsNormalized(NormalizationForm.FormC));
        }
    }

    [InlineData("./어린양.txt")]
    [InlineData("./Western/Áçčèñţşůşîñģdïäçřïţïçš.txt")]
    [Theory]
    public async Task FormDFileSystem_File_Should_Be_Normalized_To_FormC(string filePath)
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormD);

        var targetParser = new CommandLineParser<FixCommand>();

        FileSystemFixtureHelpers.ShowFilesInFileSystem(fixture, _output);

        var originalFilename = FileSystemFixtureHelpers.NormalizePathFilenameOnly(fixture, filePath, NormalizationForm.FormD);

        var t = new CancellationTokenSource();

        // Act
        var c = targetParser.ParseWithErrorHandling([originalFilename, "-q"]);

        var r = await c!.RunAsync(fixture.FileSystem, t.Token);

        // Assertion
        var filename = fixture.FileSystem!.Path.GetFileName(filePath);
        Assert.True(filename.IsNormalized(NormalizationForm.FormC));
    }
}
