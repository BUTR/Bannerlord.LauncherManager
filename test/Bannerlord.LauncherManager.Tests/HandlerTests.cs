using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using FetchBannerlordVersion;

using Microsoft.Win32.SafeHandles;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Tests;

public class LauncherManagerHandlerExposer : LauncherManagerHandler
{
    public LauncherManagerHandlerExposer(
        ILauncherStateProvider launcherStateUProvider,
        IGameInfoProvider gameInfoProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProviderProvider,
        INotificationProvider notificationProviderProvider,
        ILoadOrderStateProvider loadOrderStateProvider) :
        base(launcherStateUProvider, gameInfoProvider, fileSystemProvider, dialogProviderProvider, notificationProviderProvider, loadOrderStateProvider)
    { }

    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetModulesAsync() => await base.GetModulesAsync();

    public override async Task<string> GetGameVersionAsync() => Fetcher.GetVersion(await GetInstallPathAsync(), Constants.TaleWorldsLibrary);
}

public class HandlerTests
{
    private record ModuleViewModel : IModuleViewModel
    {
        public required ModuleInfoExtendedWithMetadata ModuleInfoExtended { get; init; }
        public required bool IsValid { get; init; }
        public required bool IsSelected { get; set; }
        public required bool IsDisabled { get; set; }
        public required int Index { get; set; }
    }

    private const string GamePath = "./Data/game/";

    private static void Read(string filePath, int offset, int length, Action<byte[]?> callback)
    {
        SafeFileHandle? fileHandle = null;
        try
        {
            fileHandle = File.OpenHandle(filePath);

            if (length == -1)
                length = (int) RandomAccess.GetLength(fileHandle);

            if (length == 0)
            {
                callback([]);
                return;
            }

            var buffer = new byte[length];
            RandomAccess.Read(fileHandle, buffer, offset);
            callback(buffer);
        }
        catch
        {
            callback(null);
        }
        finally
        {
            fileHandle?.Dispose();
        }
    }

    [Test]
    public async Task Sorter_Sort_Test()
    {
        var loadOrder = new LoadOrder
        {
            {"Test2", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 0 }},
            {"Test", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 1 }},
        };
        var expectedLoadOrderIds = new[] { "Test", "Test2" };

        var moduleViewModels = Array.Empty<IModuleViewModel>();
        var setModuleViewModels = default(IReadOnlyList<IModuleViewModel>)!;

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(Path.GetFullPath(GamePath)!)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters, callback) => callback(),
                getOptions: null!,
                getState: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: callback => callback(moduleViewModels),
                getModuleViewModels: callback => callback(moduleViewModels),
                setModuleViewModels: (x, callback) => { setModuleViewModels = x; callback(); })
        );

        var modules = await handler.GetModulesAsync();
        moduleViewModels =
        [
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 0,
            },
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test2"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 1,
            },
        ];

        await handler.SortAsync();

        Assert.That(setModuleViewModels.Select(x => x.ModuleInfoExtended.Id).ToArray(), Is.EqualTo(expectedLoadOrderIds));
    }

    [Test]
    public async Task OrderBy_Test()
    {
        var loadOrder = new LoadOrder
        {
            {"Test2", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 0 }},
            {"Test", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 1 }},
        };
        var expectedLoadOrderIds = new[] { "Test", "Test2" };

        var moduleViewModels = Array.Empty<IModuleViewModel>();

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(Path.GetFullPath(GamePath)!)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters, callback) => callback(),
                getOptions: callback => callback(new LauncherOptions(false)),
                getState: callback => callback(new LauncherState(true))
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: callback => callback(moduleViewModels),
                getModuleViewModels: callback => callback(moduleViewModels),
                setModuleViewModels: null!)
        );

        await handler.RefreshModulesAsync();
        var modules = await handler.GetModulesAsync();
        moduleViewModels =
        [
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 0,
            },
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test2"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 1,
            },
        ];

        var (result, issues, sorted) = await handler.TryOrderByLoadOrderAsync(loadOrder.Keys, x => true);

        Assert.That(sorted.Select(x => x.ModuleInfoExtended.Id).ToArray(), Is.EqualTo(expectedLoadOrderIds));
    }

    [Test]
    public async Task OrderBy_Beta_RejectsInvalidUserOrder_Test()
    {
        // User saved [Test2, Test] but Test2 depends on Test. Beta is strict: it must surface
        // the dependency violation rather than silently auto-correct (which is what TW does).
        var loadOrder = new LoadOrder
        {
            {"Test2", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 0 }},
            {"Test", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 1 }},
        };

        var moduleViewModels = Array.Empty<IModuleViewModel>();

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(Path.GetFullPath(GamePath)!)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters, callback) => callback(),
                getOptions: callback => callback(new LauncherOptions(true)),
                getState: callback => callback(new LauncherState(true))
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: callback => callback(moduleViewModels),
                getModuleViewModels: callback => callback(moduleViewModels),
                setModuleViewModels: null!)
        );

        await handler.RefreshModulesAsync();
        var modules = await handler.GetModulesAsync();
        moduleViewModels =
        [
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 0,
            },
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test2"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 1,
            },
        ];

        var (result, issues, _) = await handler.TryOrderByLoadOrderAsync(loadOrder.Keys, x => true);

        Assert.That(result, Is.False);
        Assert.That(issues, Is.Not.Null);
        Assert.That(issues, Is.Not.Empty);
    }

    [Test]
    public async Task OrderBy_Beta_AcceptsValidUserOrder_Test()
    {
        // User saved [Test, Test2] which respects Test2's dependency on Test — Beta should pass.
        var loadOrder = new LoadOrder
        {
            {"Test", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 0 }},
            {"Test2", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, IsDisabled = false, Index = 1 }},
        };
        var expectedLoadOrderIds = new[] { "Test", "Test2" };

        var moduleViewModels = Array.Empty<IModuleViewModel>();

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(Path.GetFullPath(GamePath)!)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters, callback) => callback(),
                getOptions: callback => callback(new LauncherOptions(true)),
                getState: callback => callback(new LauncherState(true))
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: callback => callback(moduleViewModels),
                getModuleViewModels: callback => callback(moduleViewModels),
                setModuleViewModels: null!)
        );

        await handler.RefreshModulesAsync();
        var modules = await handler.GetModulesAsync();
        moduleViewModels =
        [
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 0,
            },
            new ModuleViewModel
            {
                ModuleInfoExtended = modules.First(x => x.Id == "Test2"),
                IsValid = true,
                IsSelected = true,
                IsDisabled = false,
                Index = 1,
            },
        ];

        var (result, _, sorted) = await handler.TryOrderByLoadOrderAsync(loadOrder.Keys, x => true);

        Assert.That(result, Is.True);
        Assert.That(sorted.Select(x => x.ModuleInfoExtended.Id).ToArray(), Is.EqualTo(expectedLoadOrderIds));
    }

    [Test]
    public async Task ModuleProvider_GetModules_Test()
    {
        const string steamInstallPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Mount & Blade II Bannerlord";
        if (!Directory.Exists(Path.Combine(steamInstallPath, Constants.ModulesFolder)))
            Assert.Ignore("Requires a local Steam installation of Bannerlord");

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(steamInstallPath)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: null!
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );

        var version = await handler.GetGameVersionAsync();

        var modules = await handler.GetModulesAsync();

        Assert.That(modules.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task ModuleProvider_VortexTaggedModule_Test()
    {
        // Vortex leaves a tag file in the module folders it deploys, the file list callback returns full paths
        var gamePath = Path.Combine(Path.GetTempPath(), $"LauncherManager_{Guid.NewGuid():N}");
        foreach (var moduleFolder in Directory.GetDirectories(Path.Combine(GamePath, Constants.ModulesFolder)))
        {
            var target = Path.Combine(gamePath, Constants.ModulesFolder, Path.GetFileName(moduleFolder));
            Directory.CreateDirectory(target);
            foreach (var file in Directory.GetFiles(moduleFolder))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        }
        File.WriteAllText(Path.Combine(gamePath, Constants.ModulesFolder, "Test2", "__folder_managed_by_vortex"), "");

        try
        {
            var handler = new LauncherManagerHandlerExposer(
                dialogProviderProvider: new CallbackDialogProvider(
                    sendDialog: null!
                ),
                fileSystemProvider: new CallbackFileSystemProvider(
                    readFileContent: Read,
                    writeFileContent: null!,
                    readDirectoryFileList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetFiles(directory) : null),
                    readDirectoryList: (directory, callback) => callback(Directory.Exists(directory) ? Directory.GetDirectories(directory) : null)
                ),
                gameInfoProvider: new CallbackGameInfoProvider(
                    getInstallPath: callback => callback(gamePath)
                ),
                notificationProviderProvider: new CallbackNotificationProvider(
                    sendNotification: (id, type, message, ms) => { }
                ),
                launcherStateUProvider: new CallbackLauncherStateProvider(
                    setGameParameters: null!,
                    getOptions: null!,
                    getState: null!
                ),
                loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                    getAllModuleViewModels: null!,
                    getModuleViewModels: null!,
                    setModuleViewModels: null!)
            );

            var modules = await handler.GetModulesAsync();

            Assert.That(modules.Count(x => x.Id == "Test"), Is.EqualTo(1));
            Assert.That(modules.Count(x => x.Id == "Test2"), Is.EqualTo(1));
            Assert.That(modules.First(x => x.Id == "Test").ModuleProviderType, Is.EqualTo(ModuleProviderType.Default));
            Assert.That(modules.First(x => x.Id == "Test2").ModuleProviderType, Is.EqualTo(ModuleProviderType.Vortex));
        }
        finally
        {
            Directory.Delete(gamePath, true);
        }
    }

    [Test]
    public async Task ModuleProvider_ReadsOverlap_Test()
    {
        // The host completes its reads later and in any order: the reads overlap, the modules keep the folders' order
        const int moduleCount = 40;
        var modulesPath = Path.Combine("Game", Constants.ModulesFolder);
        var moduleFolders = Enumerable.Range(0, moduleCount).Select(i => Path.Combine(modulesPath, $"M{i:00}")).ToArray();
        var pending = new List<(string FilePath, Action<byte[]?> Callback)>();

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: (filePath, _, _, callback) => pending.Add((filePath, callback)),
                writeFileContent: null!,
                readDirectoryFileList: (directory, callback) => callback([Path.Combine(directory, Constants.SubModuleName)]),
                readDirectoryList: (directory, callback) => callback(directory == modulesPath ? moduleFolders : null)
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback("Game")
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );

        var modulesTask = handler.GetModulesAsync();
        var mostInFlight = 0;
        while (!modulesTask.IsCompleted)
        {
            Assert.That(pending, Is.Not.Empty, "Waits for a read that wasn't started");
            mostInFlight = Math.Max(mostInFlight, pending.Count);
            var reads = pending.ToArray();
            pending.Clear();
            foreach (var (filePath, callback) in reads.Reverse())
            {
                var id = Path.GetFileName(Path.GetDirectoryName(filePath));
                callback(System.Text.Encoding.UTF8.GetBytes($"<Module><Id value=\"{id}\"/><Name value=\"{id}\"/><Version value=\"v1.0.0\"/></Module>"));
            }
        }
        var modules = await modulesTask;

        Assert.That(mostInFlight, Is.GreaterThan(1).And.LessThanOrEqualTo(16));
        Assert.That(modules.Select(x => x.Id), Is.EqualTo(moduleFolders.Select(Path.GetFileName)));
    }

    [Test]
    public void Handler_TestModule_tTest()
    {
        var moduleFolder = "Test\\";
        var subModuleFile = "Test\\SubModule.xml";
        var files = new[]
        {
            moduleFolder,
            subModuleFile,
        };

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: null!,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: null!
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: null!
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: null!
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );

        var installResult = handler.TestModuleContent(files);
        Assert.That(installResult, Is.Not.Null);
        Assert.That(installResult.Supported, Is.True);
    }

    [Test]
    public void Handler_InstallModule_Test()
    {
        var moduleInfo = new ModuleInfoExtended
        {
            Id = "Test"
        };

        var moduleFolder = "Test\\";
        var subModuleFile = "Test\\SubModule.xml";
        var win64Dll = $"Test\\bin\\{Constants.Win64Configuration}\\Test.dll";
        var xboxDll = $"Test\\bin\\{Constants.XboxConfiguration}\\Test.dll";
        var prefab = $"Test\\Prefabs\\prefab.xml";
        var files = new[]
        {
            moduleFolder,
            subModuleFile,
            win64Dll,
            xboxDll,
            prefab,
        };

        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: null!
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: callback => callback(Path.GetFullPath(GamePath))
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: null!
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );

        handler.SetGameStore(GameStore.Steam);
        var installResult = handler.InstallModuleContent(files, [new(moduleInfo, ModuleProviderType.Default, subModuleFile)]);
        Assert.That(installResult, Is.Not.Null);
        Assert.That(installResult.Instructions, Is.Not.Null);
        Assert.That(installResult.Instructions.Count, Is.EqualTo(7));
        Assert.That(installResult.Instructions[0], Is.InstanceOf<CopyInstallInstruction>());
        Assert.That(installResult.Instructions[1], Is.InstanceOf<CopyInstallInstruction>());
        Assert.That(installResult.Instructions[2], Is.InstanceOf<CopyStoreInstallInstruction>());
        Assert.That(installResult.Instructions[3], Is.InstanceOf<CopyStoreInstallInstruction>());
        Assert.That(installResult.Instructions[4], Is.InstanceOf<CopyStoreInstallInstruction>());
        Assert.That(installResult.Instructions[5], Is.InstanceOf<CopyStoreInstallInstruction>());
        Assert.That(installResult.Instructions[6], Is.InstanceOf<ModuleInfoInstallInstruction>());
        Assert.That(((CopyInstallInstruction) installResult.Instructions[0]).Source, Is.EqualTo(subModuleFile));
        Assert.That(((CopyInstallInstruction) installResult.Instructions[1]).Source, Is.EqualTo(prefab));
        Assert.That(((CopyStoreInstallInstruction) installResult.Instructions[2]).Source, Is.EqualTo(win64Dll));
        Assert.That(((CopyStoreInstallInstruction) installResult.Instructions[3]).Source, Is.EqualTo(win64Dll));
        Assert.That(((CopyStoreInstallInstruction) installResult.Instructions[4]).Source, Is.EqualTo(win64Dll));
        Assert.That(((CopyStoreInstallInstruction) installResult.Instructions[5]).Source, Is.EqualTo(xboxDll));
        Assert.That(((ModuleInfoInstallInstruction) installResult.Instructions[6]).ModuleInfo, Is.EqualTo(moduleInfo));
    }
}