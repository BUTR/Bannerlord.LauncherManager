using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using BUTR.NativeAOT.Shared;

using FetchBannerlordVersion;

using Mono.Cecil;
using Mono.Cecil.Rocks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

internal sealed class LauncherManagerHandlerNative : LauncherManagerHandler, IDisposable
{
    private static readonly string SavePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Game Saves");

    public static unsafe LauncherManagerHandlerNative? FromPointer(void* ptr) => GCHandle.FromIntPtr(new IntPtr(ptr)).Target as LauncherManagerHandlerNative;

    public unsafe param_ptr* OwnerPtr { get; }
    public unsafe VoidPtr* HandlePtr { get; }

    public unsafe LauncherManagerHandlerNative(param_ptr* pOwner,
        ILauncherStateProvider launcherStateUProvider,
        IGameInfoProvider gameInfoProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProvider,
        INotificationProvider notificationProvider,
        ILoadOrderStateProvider loadOrderStateProvider) :
        base(launcherStateUProvider, gameInfoProvider, fileSystemProvider, dialogProvider, notificationProvider, loadOrderStateProvider)
    {
        OwnerPtr = pOwner;
        HandlePtr = (VoidPtr*) GCHandle.ToIntPtr(GCHandle.Alloc(this, GCHandleType.Normal)).ToPointer();
    }

    private unsafe void ReleaseUnmanagedResources()
    {
        var handle = GCHandle.FromIntPtr(new IntPtr(HandlePtr));
        if (handle.IsAllocated) handle.Free();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~LauncherManagerHandlerNative()
    {
        ReleaseUnmanagedResources();
    }

    public new async Task SetGameParameterLoadOrderAsync(LoadOrder loadOrder) => await base.SetGameParameterLoadOrderAsync(loadOrder);


    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetModulesAsync() => await base.GetModulesAsync();
    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetAllModulesAsync() => await base.GetAllModulesAsync();

    public new async Task<IEnumerable<IModuleViewModel>?> GetModuleViewModelsAsync() => await base.GetModuleViewModelsAsync();

    public new async Task ShowHintAsync(BUTRTextObject message) => await base.ShowHintAsync(message);
    public new async Task ShowHintAsync(string message) => await base.ShowHintAsync(message);

    public new async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters) => await base.SendDialogAsync(type, title, message, filters);

    public override Task<SaveMetadata?> GetSaveMetadataAsync(string fileName, ReadOnlyMemory<byte> data)
    {
        var length = BitConverter.ToInt32(data.Slice(0, 4).Span);
        if (length > 5 * 1024 * 1024) return Task.FromResult<SaveMetadata?>(null); // 5MB JSON? Orly?
        if (length > data.Length - 4) return Task.FromResult<SaveMetadata?>(null);

        try
        {
            var metadata = JsonSerializer.Deserialize(Encoding.UTF8.GetString(data.Slice(4, length).Span), Bindings.CustomSourceGenerationContext.TWSaveMetadata);
            if (metadata == null) return Task.FromResult<SaveMetadata?>(null);
            return Task.FromResult<SaveMetadata?>(new SaveMetadata(Path.GetFileName(fileName), metadata));
        }
        catch (JsonException)
        {
            return Task.FromResult<SaveMetadata?>(null);
        }
    }

    public override async Task<IReadOnlyList<SaveMetadata>> GetSaveFilesAsync()
    {
        async IAsyncEnumerable<SaveMetadata> GetSaveFilesInternal()
        {
            foreach (var filePath in await ReadDirectoryFileListAsync(SavePath) ?? [])
            {
                if (await ReadFileContentAsync(filePath, 0, 4) is not { } lengthData) continue;
                var length = BitConverter.ToInt32(lengthData, 0);
                if (length > 5 * 1024 * 1024) continue; // 5MB JSON? Orly?
                if (await ReadFileContentAsync(filePath, 4, length) is not { } jsonData) continue;

                SaveMetadata? saveMetadata;
                try
                {
                    var metadata = JsonSerializer.Deserialize(jsonData, Bindings.CustomSourceGenerationContext.TWSaveMetadata);
                    saveMetadata = metadata is not null ? new SaveMetadata(Path.GetFileNameWithoutExtension(filePath), metadata) : null;
                }
                catch (JsonException) { saveMetadata = null; }
                if (saveMetadata is null) continue;

                yield return saveMetadata;
            }
        }
        return await GetSaveFilesInternal().ToArrayAsync();
    }

    public override Task<string?> GetSaveFilePathAsync(string saveFile)
    {
        return Task.FromResult<string?>(Path.Combine(SavePath, $"{saveFile}.sav"));
    }


    public override async Task<string> GetGameVersionAsync()
    {
        var gamePath = await GetInstallPathAsync();
        return Fetcher.GetVersion(gamePath, Constants.TaleWorldsLibrary);
    }

    public override async Task<int> GetChangesetAsync()
    {
        var gamePath = await GetInstallPathAsync();
        return Fetcher.GetChangeSet(gamePath, Constants.TaleWorldsLibrary);
    }

    public async Task<bool> IsObfuscatedAsync(ModuleInfoExtendedWithMetadata moduleInfoExtended)
    {
        static bool CanBeLoaded(SubModuleInfoExtended x) =>
            true;
        //ModuleInfoHelper.CheckIfSubModuleCanBeLoaded(x, ApplicationPlatform.CurrentPlatform, ApplicationPlatform.CurrentRuntimeLibrary, TaleWorlds.MountAndBlade.DedicatedServerType.None, false);

        bool IsObfuscatedInternal(byte[] data)
        {
            try
            {
                using var stream = new MemoryStream(data);
                using var moduleDefinition = ModuleDefinition.ReadModule(stream, new ReaderParameters(ReadingMode.Deferred));

                var hasObfuscationAttributeUsed = moduleDefinition.GetCustomAttributes().Any(x => x.Constructor.DeclaringType.Name switch
                {
                    "ConfusedByAttribute" => true,
                    _ => false,
                });
                var hasObfuscationAttributeDeclared = moduleDefinition.Types.Any(x => x.Name switch
                {
                    "ConfusedByAttribute" => true,
                    _ => false,
                });
                // Every module should have a module initializer. If it's missing, someone is hiding it
                var hasModuleInitializer = moduleDefinition.GetAllTypes().Any(x => x.Name == "<Module>");

                // The body will not be readable if the code is obfuscated
                _ = moduleDefinition.GetAllTypes().SelectMany(x => x.Methods).Where(x => x.HasBody).Sum(x => x.Body.Instructions.Count);

                return hasObfuscationAttributeUsed || hasObfuscationAttributeDeclared || !hasModuleInitializer;
            }
            // Failing to read the metadata is a direct sign of metadata manipulation
            catch (Exception e)
            {
                Logger.LogException(e);
                return true;
            }
        }

        var platform = await GetPlatformAsync();
        var moduleDir = Path.GetDirectoryName(moduleInfoExtended.Path)!;
        foreach (var subModule in moduleInfoExtended.SubModules.Where(CanBeLoaded))
        {
            var asmWin = Path.Combine(moduleDir, Constants.BinFolder, Constants.Win64Configuration, subModule.DLLName);
            var asmXbox = Path.Combine(moduleDir, Constants.BinFolder, Constants.XboxConfiguration, subModule.DLLName);
            switch (platform)
            {
                case GamePlatform.Win64:
                {
                    if (await FileSystemProvider.ReadFileContentAsync(asmWin, 0, -1) is { } dataWin && IsObfuscatedInternal(dataWin))
                        return true;
                    break;
                }
                case GamePlatform.Xbox:
                {
                    if (await FileSystemProvider.ReadFileContentAsync(asmXbox, 0, -1) is { } dataXbox && IsObfuscatedInternal(dataXbox))
                        return true;

                    // If we use Win64 binaries on Xbox
                    if (await FileSystemProvider.ReadFileContentAsync(asmWin, 0, -1) is { } dataWin && IsObfuscatedInternal(dataWin))
                        return true;

                    break;
                }
            }
        }

        return false;
    }
}