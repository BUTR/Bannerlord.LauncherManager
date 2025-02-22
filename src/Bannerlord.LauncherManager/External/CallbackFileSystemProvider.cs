using System;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackFileSystemProvider : IFileSystemProvider
{
    private readonly Action<string, int, int, Action<byte[]?>> _readFileContent;
    private readonly Action<string, byte[]?, Action> _writeFileContent;
    private readonly Action<string, Action<string[]?>> _readDirectoryFileList;
    private readonly Action<string, Action<string[]?>> _readDirectoryList;

    public CallbackFileSystemProvider(
        Action<string, int, int, Action<byte[]?>> readFileContent,
        Action<string, byte[]?, Action> writeFileContent,
        Action<string, Action<string[]?>> readDirectoryFileList,
        Action<string, Action<string[]?>> readDirectoryList)
    {
        _readFileContent = readFileContent;
        _writeFileContent = writeFileContent;
        _readDirectoryFileList = readDirectoryFileList;
        _readDirectoryList = readDirectoryList;
    }
    
    public async Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length)
    {
        var tcs = new TaskCompletionSource<byte[]?>();
        _readFileContent(filePath, offset, length, (result) => tcs.TrySetResult(result));
        return await tcs.Task;
    }
    
    public async Task WriteFileContentAsync(string filePath, byte[]? data)
    {
        var tcs = new TaskCompletionSource<object?>();
        _writeFileContent(filePath, data, () => tcs.TrySetResult(null));
        await tcs.Task;
    }
    
    public async Task<string[]?> ReadDirectoryFileListAsync(string directoryPath)
    {
        var tcs = new TaskCompletionSource<string[]?>();
        _readDirectoryFileList(directoryPath, (result) => tcs.TrySetResult(result));
        return await tcs.Task;
    }
    
    public async Task<string[]?> ReadDirectoryListAsync(string directoryPath)
    {
        var tcs = new TaskCompletionSource<string[]?>();
        _readDirectoryList(directoryPath, (result) => tcs.TrySetResult(result));
        return await tcs.Task;
    }
}