using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

using System;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class FileSystemProvider : IFileSystemProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_ReadFileContentDelegate _readFileContent;
    private readonly N_WriteFileContentDelegate _writeFileContent;
    private readonly N_ReadDirectoryFileList _readDirectoryFileList;
    private readonly N_ReadDirectoryList _readDirectoryList;

    public FileSystemProvider(
        param_ptr* pOwner,
        N_ReadFileContentDelegate readFileContent,
        N_WriteFileContentDelegate writeFileContent,
        N_ReadDirectoryFileList readDirectoryFileList,
        N_ReadDirectoryList readDirectoryList)
    {
        _pOwner = pOwner;
        _readFileContent = readFileContent;
        _writeFileContent = writeFileContent;
        _readDirectoryFileList = readDirectoryFileList;
        _readDirectoryList = readDirectoryList;
    }

    public byte[]? ReadFileContent(string filePath, int offset, int length) => ReadFileContentNative(filePath, offset, length);
    public void WriteFileContent(string filePath, byte[]? data) => WriteFileContentNative(filePath, data, data?.Length ?? 0);
    public string[]? ReadDirectoryFileList(string directoryPath) => ReadDirectoryFileListNative(directoryPath);
    public string[]? ReadDirectoryList(string directoryPath) => ReadDirectoryListNative(directoryPath);

    private byte[]? ReadFileContentNative(ReadOnlySpan<char> filePath, int offset, int length)
    {
        Logger.LogInput(offset, length);

        fixed (char* pFilePath = filePath)
        {
            Logger.LogPinned(pFilePath);

            using var result = SafeStructMallocHandle.Create(_readFileContent(_pOwner, (param_string*) pFilePath, offset, length), true);
            using var content = result.ValueAsData();
            if (content.IsInvalid) return null;

            var returnResult = content.ToSpan().ToArray();
            Logger.LogOutput(returnResult, nameof(ReadFileContent));
            return returnResult;
        }
    }

    private void WriteFileContentNative(ReadOnlySpan<char> filePath, ReadOnlySpan<byte> data, int length)
    {
        Logger.LogInput(length);

        fixed (char* pFilePath = filePath)
        fixed (byte* pData = data)
        {
            Logger.LogPinned(pFilePath);

            using var result = SafeStructMallocHandle.Create(_writeFileContent(_pOwner, (param_string*) pFilePath, (param_data*) pData, length), true);
            result.ValueAsVoid();
        }
        Logger.LogOutput();
    }

    private string[]? ReadDirectoryFileListNative(ReadOnlySpan<char> directoryPath)
    {
        Logger.LogInput();

        fixed (char* pDirectoryPath = directoryPath)
        {
            Logger.LogPinned(pDirectoryPath);

            using var result = SafeStructMallocHandle.Create(_readDirectoryFileList(_pOwner, (param_string*) pDirectoryPath), true);
            if (result.IsNull) return null;

            var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.StringArray);
            Logger.LogOutput(returnResult);
            return returnResult;
        }
    }

    private string[]? ReadDirectoryListNative(ReadOnlySpan<char> directoryPath)
    {
        Logger.LogInput();

        fixed (char* pDirectoryPath = directoryPath)
        {
            Logger.LogPinned(pDirectoryPath);

            using var result = SafeStructMallocHandle.Create(_readDirectoryList(_pOwner, (param_string*) pDirectoryPath), true);
            if (result.IsNull) return null;

            var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.StringArray);
            Logger.LogOutput(returnResult);
            return returnResult;
        }
    }
}