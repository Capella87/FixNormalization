using Xunit;
using Xunit.Abstractions;
using NSubstitute;
using FluentAssertions;
using System.Text;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using FixNormalization.Commands;

namespace FixNormalization.Tests;

public class FormDNormalizationTests : IClassFixture<FileSystemFixture>
{
    FileSystemFixture fixture;

    public FormDNormalizationTests(FileSystemFixture fixture)
    {
        this.fixture = fixture;
    }

    [InlineData(".\\FirstSubDir\\옛한글")]
    [InlineData(".\\FirstSubDir")]
    [Theory]
    public async Task FormDFileSystem_Should_Be_Normalized_To_FormC_NonRecursive(string directoryPath)
    {
        // Arrange
        FileSystemFixtureHelpers.ConfigureMockData(fixture, NormalizationForm.FormD);

        var targetParser = new CommandLineParser<FixCommand>();

        // Act
        var c = targetParser.ParseWithErrorHandling([directoryPath]);

        var r = await c!.RunAsync(fixture.FileSystem);

        // Assertion
        foreach (var f in this.fixture.FileSystem.Directory.GetFiles(directoryPath))
        {
            var filename = fixture.FileSystem.Path.GetFileName(f);

            filename.IsNormalized(NormalizationForm.FormC).Should().BeTrue();
        }

        // Assert for files in subdirectories
        Assert.All(this.fixture.FileSystem.Directory.GetDirectories(directoryPath),
            (f) => fixture.FileSystem.Directory.GetFiles(f, "", SearchOption.AllDirectories)
            .All(filePath => !(Path.GetFileName(filePath)
            .IsNormalized(NormalizationForm.FormC))));
    }
}