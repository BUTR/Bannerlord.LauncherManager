namespace Bannerlord.LauncherManager.External;

public interface IFileSystemProvider
{
    byte[]? ReadFileContent(string filePath, int offset, int length);
    void WriteFileContent(string filePath, byte[]? data);
    string[]? ReadDirectoryFileList(string directoryPath);
    string[]? ReadDirectoryList(string directoryPath);
}