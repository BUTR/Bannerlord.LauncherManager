using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using Mono.Cecil;
using Mono.Cecil.Rocks;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

partial class LauncherManagerHandlerNative
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [Flags]
    private enum SubModulePlatform
    {
        Undefined,
        WindowsSteam,
        WindowsEpic,
        Orbis,
        Durango,
        Web,
        WindowsNoPlatform,
        LinuxNoPlatform,
        WindowsGOG,
        GDKDesktop,
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum SubModuleRuntime
    {
        Mono,
        DotNet,
        DotNetCore,
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum SubModuleDedicatedServerType
    {
        None,
        Matchmaker,
        Custom,
        Community,
    }
    private static bool GetSubModuleTagValidity(SubModuleTags tag, string value, SubModulePlatform cPlatform, SubModuleRuntime cRuntime, SubModuleDedicatedServerType cServerType, bool playerHostedDedicatedServer) => tag switch
    {
        SubModuleTags.RejectedPlatform => !Enum.TryParse<SubModulePlatform>(value, out var platform) || !cPlatform.HasFlag(platform),
        SubModuleTags.ExclusivePlatform => !Enum.TryParse<SubModulePlatform>(value, out var platform) || cPlatform.HasFlag(platform),
        // We don't know the runtime, since we support all of them
        //SubModuleTags.DependantRuntimeLibrary => !Enum.TryParse<SubModuleRuntime>(value, out var runtime) || cRuntime == runtime,
        SubModuleTags.IsNoRenderModeElement => value.Equals("false"),
        SubModuleTags.DedicatedServerType => value.ToLower() switch
        {
            "none" => cServerType == SubModuleDedicatedServerType.None,
            "both" => cServerType == SubModuleDedicatedServerType.None,
            "all" => cServerType == SubModuleDedicatedServerType.None,
            "custom" => cServerType == SubModuleDedicatedServerType.Custom,
            "matchmaker" => cServerType == SubModuleDedicatedServerType.Matchmaker,
            "community" => cServerType == SubModuleDedicatedServerType.Community,
            _ => false,
        },
        SubModuleTags.PlayerHostedDedicatedServer => playerHostedDedicatedServer && value.Equals("true"),
        _ => true,
    };
    private async Task<SubModulePlatform> GetSubModulePlatform() => await GetPlatformAsync() switch
    {
        GamePlatform.Win64 => SubModulePlatform.WindowsSteam |
                              SubModulePlatform.WindowsEpic |
                              SubModulePlatform.WindowsNoPlatform |
                              SubModulePlatform.WindowsGOG,
        GamePlatform.Xbox => SubModulePlatform.GDKDesktop |
                             SubModulePlatform.Durango,
        _ => SubModulePlatform.Undefined,
    };
    private Task<SubModuleRuntime> GetSubModuleRuntime() => Task.FromResult((SubModuleRuntime) 0);
    private Task<SubModuleDedicatedServerType> GetSubModuleDedicatedServerType() => Task.FromResult(SubModuleDedicatedServerType.None);

    private static bool CheckIfSubModuleCanBeLoaded(SubModuleInfoExtended subModuleInfo, SubModulePlatform cSubModulePlatform, SubModuleRuntime cSubModuleRuntime, SubModuleDedicatedServerType cServerType, bool playerHostedDedicatedServer)
    {
        if (subModuleInfo.Tags.Count <= 0) return true;

        foreach (var (key, values) in subModuleInfo.Tags)
        {
            if (!Enum.TryParse<SubModuleTags>(key, out var tag))
                continue;

            if (values.Any(value => !GetSubModuleTagValidity(tag, value, cSubModulePlatform, cSubModuleRuntime, cServerType, playerHostedDedicatedServer)))
                return false;
        }
        return true;
    }

    public async Task<bool> IsObfuscatedAsync(ModuleInfoExtendedWithMetadata moduleInfoExtended)
    {
        var subModulePlatform = await GetSubModulePlatform();
        var subModuleRuntime = await GetSubModuleRuntime();
        var subModuleServerType = await GetSubModuleDedicatedServerType();

        bool CanBeLoaded(SubModuleInfoExtended x) => CheckIfSubModuleCanBeLoaded(x, subModulePlatform, subModuleRuntime, subModuleServerType, false);

        bool IsObfuscatedInternal(byte[] data)
        {
#if DEBUG
            using var logger = LogMethod();
#else
            using var logger = LogMethod();
#endif

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
                logger.LogException(e);
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