using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed class FileSystemProvider : IFileSystemProvider
{
    private readonly unsafe param_ptr* _pOwner;
    private readonly N_ReadFileContentDelegate _readFileContent;
    private readonly N_WriteFileContentDelegate _writeFileContent;
    private readonly N_ReadDirectoryFileList _readDirectoryFileList;
    private readonly N_ReadDirectoryList _readDirectoryList;

    public unsafe FileSystemProvider(
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

    public async Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length)
    {
        var tcs = new TaskCompletionSource<byte[]?>();
        ReadFileContentNative(filePath, offset, length, tcs);
        return await tcs.Task;
    }

    public async Task WriteFileContentAsync(string filePath, byte[]? data)
    {
        var tcs = new TaskCompletionSource();
        WriteFileContentNative(filePath, data, data?.Length ?? 0, tcs);
        await tcs.Task;
    }

    public async Task<string[]?> ReadDirectoryFileListAsync(string directoryPath)
    {
        var tcs = new TaskCompletionSource<string[]?>();
        ReadDirectoryFileListNative(directoryPath, tcs);
        return await tcs.Task;
    }

    public async Task<string[]?> ReadDirectoryListAsync(string directoryPath)
    {
        var tcs = new TaskCompletionSource<string[]?>();
        ReadDirectoryListNative(directoryPath, tcs);
        return await tcs.Task;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadFileContentNativeCallback(param_ptr* pOwner, return_value_data* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<byte[]?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var hResult = SafeStructMallocHandle.Create(pResult, true);
        hResult.SetAsData(tcs);
        handle.Free();

        Logger.LogOutput();
    }
    private unsafe void ReadFileContentNative(ReadOnlySpan<char> filePath, int offset, int length, TaskCompletionSource<byte[]?> tcs)
    {
        Logger.LogInput(offset, length);

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pFilePath = filePath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readFileContent(_pOwner, (param_string*) pFilePath, offset, length, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadFileContentNativeCallback), true);
                result.ValueAsVoid();

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void WriteFileContentNativeCallback(param_ptr* pOwner, return_value_void* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsVoid(tcs);
        handle.Free();

        Logger.LogOutput();
    }
    private unsafe void WriteFileContentNative(ReadOnlySpan<char> filePath, ReadOnlySpan<byte> data, int length, TaskCompletionSource tcs)
    {
        Logger.LogInput(length);

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pFilePath = filePath)
        fixed (byte* pData = data)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_writeFileContent(_pOwner, (param_string*) pFilePath, (param_data*) pData, length, (param_ptr*) GCHandle.ToIntPtr(handle), &WriteFileContentNativeCallback), true);
                result.ValueAsVoid();

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
        Logger.LogOutput();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadDirectoryFileListNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string[]?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.StringArray);
        handle.Free();

        Logger.LogOutput();
    }
    private unsafe void ReadDirectoryFileListNative(ReadOnlySpan<char> directoryPath, TaskCompletionSource<string[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pDirectoryPath = directoryPath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readDirectoryFileList(_pOwner, (param_string*) pDirectoryPath, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadDirectoryFileListNativeCallback), true);
                result.ValueAsVoid();

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadDirectoryListNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string[]?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.StringArray);
        handle.Free();

        Logger.LogOutput();
    }
    private unsafe void ReadDirectoryListNative(string directoryPath, TaskCompletionSource<string[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pDirectoryPath = directoryPath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readDirectoryList(_pOwner, (param_string*) pDirectoryPath, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadDirectoryListNativeCallback), true);
                result.ValueAsVoid();

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }
}