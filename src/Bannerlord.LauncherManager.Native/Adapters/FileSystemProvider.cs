using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Native.Extensions;

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

    public Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length)
    {
#if DEBUG
        using var logger = LogMethod(filePath.ToFormattable(), offset, length);
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<byte[]?>();
            ReadFileContentNative(filePath, offset, length, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    public Task WriteFileContentAsync(string filePath, byte[]? data)
    {
#if DEBUG
        using var logger = LogMethod(filePath.ToFormattable(), data?.Length ?? 0);
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource();
            WriteFileContentNative(filePath, data, data?.Length ?? 0, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    public Task<string[]?> ReadDirectoryFileListAsync(string directoryPath)
    {
#if DEBUG
        using var logger = LogMethod(directoryPath.ToFormattable());
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<string[]?>();
            ReadDirectoryFileListNative(directoryPath, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    public Task<string[]?> ReadDirectoryListAsync(string directoryPath)
    {
#if DEBUG
        using var logger = LogMethod(directoryPath.ToFormattable());
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<string[]?>();
            ReadDirectoryListNative(directoryPath, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadFileContentNativeCallback(param_ptr* pOwner, return_value_data* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod();
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<byte[]?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsData(tcs);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void ReadFileContentNative(ReadOnlySpan<char> filePath, int offset, int length, TaskCompletionSource<byte[]?> tcs)
    {
#if DEBUG
        using var logger = LogMethod(offset, length);
#else
        using var logger = LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pFilePath = filePath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readFileContent(_pOwner, (param_string*) pFilePath, offset, length, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadFileContentNativeCallback), true);
                logger.LogResult(result);
                result.ValueAsVoid();
            }
            catch (Exception e)
            {
                logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void WriteFileContentNativeCallback(param_ptr* pOwner, return_value_void* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod();
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsVoid(tcs);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void WriteFileContentNative(ReadOnlySpan<char> filePath, ReadOnlySpan<byte> data, int length, TaskCompletionSource tcs)
    {
#if DEBUG
        using var logger = LogMethod(length);
#else
        using var logger = LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pFilePath = filePath)
        fixed (byte* pData = data)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_writeFileContent(_pOwner, (param_string*) pFilePath, (param_data*) pData, length, (param_ptr*) GCHandle.ToIntPtr(handle), &WriteFileContentNativeCallback), true);
                logger.LogResult(result);
                result.ValueAsVoid();
            }
            catch (Exception e)
            {
                logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadDirectoryFileListNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod();
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string[]?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.StringArray);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void ReadDirectoryFileListNative(ReadOnlySpan<char> directoryPath, TaskCompletionSource<string[]?> tcs)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pDirectoryPath = directoryPath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readDirectoryFileList(_pOwner, (param_string*) pDirectoryPath, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadDirectoryFileListNativeCallback), true);
                logger.LogResult(result);
                result.ValueAsVoid();
            }
            catch (Exception e)
            {
                logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void ReadDirectoryListNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod();
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string[]?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.StringArray);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void ReadDirectoryListNative(string directoryPath, TaskCompletionSource<string[]?> tcs)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pDirectoryPath = directoryPath)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_readDirectoryList(_pOwner, (param_string*) pDirectoryPath, (param_ptr*) GCHandle.ToIntPtr(handle), &ReadDirectoryListNativeCallback), true);
                logger.LogResult(result);
                result.ValueAsVoid();
            }
            catch (Exception e)
            {
                logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }
}