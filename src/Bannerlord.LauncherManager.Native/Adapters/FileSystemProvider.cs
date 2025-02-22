using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
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
        var tcs = new TaskCompletionSource<object?>();
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
    public static unsafe void ReadFileContentNativeCallback(param_ptr* pOwner, param_data* pResult, param_int pResultLength)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<byte[]?>);
        try
        {
            var result = param_data.ToSpan(pResult, pResultLength).ToArray();

            tcs = (TaskCompletionSource<byte[]?>) handle.Target!;
            tcs.TrySetResult(result);
            
            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs?.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void ReadFileContentNative(ReadOnlySpan<char> filePath, int offset, int length, TaskCompletionSource<byte[]?> tcs)
    {
        Logger.LogInput(offset, length);

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
        fixed (char* pFilePath = filePath)
        {
            Logger.LogPinned(pFilePath);

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
    public static unsafe void WriteFileContentNativeCallback(param_ptr* pOwner)
    {
        Logger.LogInput(pOwner);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<object?>);
        try
        {
            tcs = (TaskCompletionSource<object?>) handle.Target!;
            tcs.TrySetResult(null);
            
            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs?.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void WriteFileContentNative(ReadOnlySpan<char> filePath, ReadOnlySpan<byte> data, int length, TaskCompletionSource<object?> tcs)
    {
        Logger.LogInput(length);

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pFilePath = filePath)
        fixed (byte* pData = data)
        {
            Logger.LogPinned(pFilePath);

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
    public static unsafe void ReadDirectoryFileListNativeCallback(param_ptr* pOwner, param_json* pResult)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<string[]?>);
        try
        {
            var result = BUTR.NativeAOT.Shared.Utils.DeserializeJson(pResult, Bindings.CustomSourceGenerationContext.StringArray)?.Where(x => x is not null).ToArray();

            tcs = (TaskCompletionSource<string[]?>) handle.Target!;
            tcs.TrySetResult(result);
            
            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs?.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void ReadDirectoryFileListNative(ReadOnlySpan<char> directoryPath, TaskCompletionSource<string[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pDirectoryPath = directoryPath)
        {
            Logger.LogPinned(pDirectoryPath);

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
    public static unsafe void ReadDirectoryListNativeCallback(param_ptr* pOwner, param_json* pResult)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<string[]?>);
        try
        {
            var result = BUTR.NativeAOT.Shared.Utils.DeserializeJson(pResult, Bindings.CustomSourceGenerationContext.StringArray)?.Where(x => x is not null).ToArray();

            tcs = (TaskCompletionSource<string[]?>) handle.Target!;
            tcs.TrySetResult(result);
            
            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs?.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void ReadDirectoryListNative(ReadOnlySpan<char> directoryPath, TaskCompletionSource<string[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
        fixed (char* pDirectoryPath = directoryPath)
        {
            Logger.LogPinned(pDirectoryPath);

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