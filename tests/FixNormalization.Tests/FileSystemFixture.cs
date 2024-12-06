using System.ComponentModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using Xunit;

namespace FixNormalization.Tests;

public class FileSystemFixture : IDisposable
{
    private IFileSystem? _fileSystem;

    public IFileSystem? FileSystem => _fileSystem;

    private bool _disposed = false;

    public FileSystemFixture()
    {
        _fileSystem = new MockFileSystem();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            ClearFileSystem();
            _fileSystem = null;
        }

        _disposed = true;
    }

    public void ClearFileSystem()
    {
        // Remove all files and directories in virtual file system

        // Check whether the file system is virtual or not
        if (_fileSystem is MockFileSystem mockFileSystem)
        {
            var root = mockFileSystem.Directory.GetCurrentDirectory();

            // Delete subdirectories including contents in the root directory
            foreach (var dir in mockFileSystem.Directory.GetDirectories(root))
            {
                mockFileSystem.Directory.Delete(dir, true);
            }

            // Delete files in the root directory
            foreach (var file in mockFileSystem.Directory.GetFiles(root))
            {
                mockFileSystem.File.Delete(file);
            }
        }
    }
}
