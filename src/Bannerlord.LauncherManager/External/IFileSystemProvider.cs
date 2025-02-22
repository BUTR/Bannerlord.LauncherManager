using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public interface IFileSystemProvider
{
    Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length);
    Task WriteFileContentAsync(string filePath, byte[]? data);
    Task<string[]?> ReadDirectoryFileListAsync(string directoryPath);
    Task<string[]?> ReadDirectoryListAsync(string directoryPath);
}