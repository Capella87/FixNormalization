using Spectre.Console;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FixNormalization.Tests;

public static class FileSystemFixtureHelpers
{
    public static bool ConfigureMockData(FileSystemFixture fixture, NormalizationForm nform = NormalizationForm.FormD)
    {
        fixture = fixture ?? throw new NullReferenceException("Fixture must have valid FileSystem");
        var fileSystem = fixture.FileSystem;


        // Try cast IFileSystem to MockFileSystem
        if (fileSystem is not MockFileSystem mockFileSystem) throw new ArgumentException($"{nameof(fileSystem)} is not MockFileSystem");

        // Delete remnants of previous test result
        fixture.ClearFileSystem();

        // Get the root directory of file system
        var root = mockFileSystem.Directory.GetCurrentDirectory();

        var directories = new string[]
        {
            "FirstSubDir", "SecondSubDir", "FirstSubDir\\옛한글"
        };
        foreach (var dir in directories)
        {
            mockFileSystem.AddDirectory(mockFileSystem.Path.Join(root, dir));
        }

        var dirs = new string[] { "FirstSubDir", "SecondSubDir", "FirstSubDir\\옛한글", "Western" };

        foreach (var d in dirs)
        {
            mockFileSystem.AddDirectory(d);
        }

        var fileDictionary = new Dictionary<string, MockFileData>()
        {
            { $"FirstSubDir\\{"환영합니다.bin".Normalize(nform)}",
                new MockFileData([0xED, 0x99, 0x98, 0xEC, 0x98, 0x81, 0xED,
                    0x95, 0xA9, 0xEB, 0x8B, 0x88, 0xEB, 0x8B, 0xA4]) },
            { $"FirstSubDir\\{"아침일찍구름낀백제성을떠나.txt".Normalize(nform)}",
                new MockFileData("아침 일찍 구름 낀 백제성을 떠나")},
            { $"FirstSubDir\\옛한글\\{"불휘기픈남ᄀᆞᆫᄇᆞᄅᆞ매아니뮐ᄊᆡ곶됴코여름하ᄂᆞ·니.txt".Normalize(nform)}",
                new MockFileData("불휘기픈남ᄀᆞᆫᄇᆞᄅᆞ매아니뮐ᄊᆡ곶됴코여름하ᄂᆞ·니") },
            { $"어린양.txt".Normalize(nform), new MockFileData("어린양") },
            { $"Western\\{"Áçčèñţşůşîñģdïäçřïţïçš.txt".Normalize(nform)}", new MockFileData("asdfweg") },
        };

        foreach (var f in fileDictionary)
        {
            mockFileSystem.AddFile(f.Key, f.Value);
        }

        return true;
    }

    public static string NormalizePathFilenameOnly(FileSystemFixture fixture, string path, NormalizationForm form)
    {
        var filenameSeparatorIdx = path.LastIndexOf(fixture.FileSystem!.Path.DirectorySeparatorChar);
        return fixture.FileSystem!.Path.Combine(path.Substring(0, filenameSeparatorIdx), fixture.FileSystem!.Path.GetFileName(path).Normalize(form));
    }

    public static void ShowFilesInFileSystem(FileSystemFixture fixture, ITestOutputHelper output)
    {
        output.WriteLine("Files in FileSystem:");
        foreach (var filename in fixture.FileSystem!.Directory.EnumerateFiles(fixture.FileSystem.Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
        {
            output.WriteLine($"{filename}");
        }
    }
}
