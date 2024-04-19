using System;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackFileSystemProvider : IFileSystemProvider
{
    private readonly Func<string, int, int, byte[]?> _readFileContent;
    private readonly Action<string, byte[]?> _writeFileContent;
    private readonly Func<string, string[]?> _readDirectoryFileList;
    private readonly Func<string, string[]?> _readDirectoryList;

    public CallbackFileSystemProvider(Func<string, int, int, byte[]?> readFileContent, Action<string, byte[]?> writeFileContent, Func<string, string[]?> readDirectoryFileList, Func<string, string[]?> readDirectoryList)
    {
        _readFileContent = readFileContent;
        _writeFileContent = writeFileContent;
        _readDirectoryFileList = readDirectoryFileList;
        _readDirectoryList = readDirectoryList;
    }

    public byte[]? ReadFileContent(string filePath, int offset, int length) => _readFileContent(filePath, offset, length);
    public void WriteFileContent(string filePath, byte[]? data) => _writeFileContent(filePath, data);
    public string[]? ReadDirectoryFileList(string directoryPath) => _readDirectoryFileList(directoryPath);
    public string[]? ReadDirectoryList(string directoryPath) => _readDirectoryList(directoryPath);
}