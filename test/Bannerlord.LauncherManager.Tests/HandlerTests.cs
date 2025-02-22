using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FetchBannerlordVersion;

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
        if (!File.Exists(filePath))
        {
            callback(null);
            return;
        }

        if (offset == 0 && length == -1)
        {
            callback(File.ReadAllBytes(filePath));
            return;
        }
        
        if (offset >= 0 && length > 0)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var data = new byte[length];
            fs.Seek(offset, SeekOrigin.Begin);
            var readLength = fs.Read(data, 0, length);
            if (readLength != length)
                throw new Exception();
            callback(data);
            return;
        }
        
        callback(null);
        return;
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
                getInstallPath: (callback) => callback(Path.GetFullPath(GamePath)!)
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters, callback) => callback(),
                getOptions: (callback) => callback(new LauncherOptions(false)),
                getState: (callback) => callback(new LauncherState(true))
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: (callback) => callback(moduleViewModels),
                getModuleViewModels: (callback) => callback(moduleViewModels),
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
    public async Task ModuleProvider_GetModules_Test()
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
                getInstallPath: (callback) => callback("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Mount & Blade II Bannerlord")
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
                getInstallPath: (callback) => callback(Path.GetFullPath(GamePath)!)
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