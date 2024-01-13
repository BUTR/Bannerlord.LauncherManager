using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Tests;

public class LauncherManagerHandlerExposer : LauncherManagerHandler
{
    public LauncherManagerHandlerExposer(
        ILauncherStateProvider launcherStateUProvider,
        IGameInfoProvider gameInfoProvider,
        ILoadOrderPersistenceProvider loadOrderPersistenceProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProviderProvider,
        INotificationProvider notificationProviderProvider,
        ILoadOrderStateProvider loadOrderStateProvider) :
        base(launcherStateUProvider, gameInfoProvider, loadOrderPersistenceProvider, fileSystemProvider, dialogProviderProvider, notificationProviderProvider, loadOrderStateProvider) { }
    
    public new IReadOnlyList<ModuleInfoExtendedWithPath> GetModules() => base.GetModules();
}

public class HandlerTests
{
    private record ModuleViewModel : IModuleViewModel
    {
        public required ModuleInfoExtendedWithPath ModuleInfoExtended { get; init; }
        public required bool IsValid { get; init; }
        public required bool IsSelected { get; set; }
        public required bool IsDisabled { get; set; }
        public required int Index { get; set; }
    }

    private const string GamePath = "./Data/game/";

    private static byte[]? Read(string filePath, int offset, int length)
    {
        if (!File.Exists(filePath)) return null;

        if (offset == 0 && length == -1)
        {
            return File.ReadAllBytes(filePath);
        }
        else if (offset >= 0 && length > 0)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var data = new byte[length];
            fs.Seek(offset, SeekOrigin.Begin);
            var readLength = fs.Read(data, 0, length);
            if (readLength != length)
                throw new Exception();
            return data;
        }
        else
        {
            return null;
        }
    }

    [Test]
    public void Sorter_Sort_Test()
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
                readDirectoryFileList: directory => Directory.Exists(directory) ? Directory.GetFiles(directory) : null,
                readDirectoryList: directory => Directory.Exists(directory) ? Directory.GetDirectories(directory) : null
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: () => Path.GetFullPath(GamePath)!
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: (id, type, message, ms) => { }
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: (executable, parameters) => { },
                getOptions: null!,
                getState: null!
            ),
            loadOrderPersistenceProvider: new CallbackLoadOrderPersistenceProvider(
                loadLoadOrder: null!,
                saveLoadOrder: lo => loadOrder = lo
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: () => moduleViewModels,
                getModuleViewModels: () => moduleViewModels,
                setModuleViewModels: null!)
        );
        
        var modules = handler.GetModules();
        moduleViewModels = new IModuleViewModel[]
        {
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
        };

        handler.Sort();

        Assert.That(loadOrder.Select(x => x.Key).ToArray(), Is.EqualTo(expectedLoadOrderIds));
    }

    [Test]
    public void ModuleProvider_GetModules_Test()
    {
        var handler = new LauncherManagerHandlerExposer(
            dialogProviderProvider: new CallbackDialogProvider(
                sendDialog: null!
            ),
            fileSystemProvider: new CallbackFileSystemProvider(
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: directory => Directory.Exists(directory) ? Directory.GetDirectories(directory) : null
            ),
            gameInfoProvider: new CallbackGameInfoProvider(
                getInstallPath: () => Path.GetFullPath(GamePath)!
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: null!
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderPersistenceProvider: new CallbackLoadOrderPersistenceProvider(
                loadLoadOrder: null!,
                saveLoadOrder: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );

        var modules = handler.GetModules().ToList();

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
            loadOrderPersistenceProvider: new CallbackLoadOrderPersistenceProvider(
                loadLoadOrder: null!,
                saveLoadOrder: null!
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
        var files = new[]
        {
            moduleFolder,
            subModuleFile,
            win64Dll,
            xboxDll,
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
                getInstallPath: () => Path.GetFullPath(GamePath)!
            ),
            notificationProviderProvider: new CallbackNotificationProvider(
                sendNotification: null!
            ),
            launcherStateUProvider: new CallbackLauncherStateProvider(
                setGameParameters: null!,
                getOptions: null!,
                getState: null!
            ),
            loadOrderPersistenceProvider: new CallbackLoadOrderPersistenceProvider(
                loadLoadOrder: null!,
                saveLoadOrder: null!
            ),
            loadOrderStateProvider: new CallbackLoadOrderStateProvider(
                getAllModuleViewModels: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!)
        );
        
        handler.SetGameStore(GameStore.Steam);
        var installResult = handler.InstallModuleContent(files, new ModuleInfoExtendedWithPath[] { new(moduleInfo, subModuleFile) });
        Assert.That(installResult, Is.Not.Null);
        Assert.That(installResult.Instructions, Is.Not.Null);
        Assert.That(installResult.Instructions.Count, Is.EqualTo(3));
        Assert.That(installResult.Instructions[0], Is.InstanceOf<CopyInstallInstruction>());
        Assert.That(installResult.Instructions[1], Is.InstanceOf<CopyInstallInstruction>());
        Assert.That(installResult.Instructions[2], Is.InstanceOf<ModuleInfoInstallInstruction>());
        Assert.That(((CopyInstallInstruction) installResult.Instructions[0]).Source, Is.EqualTo(subModuleFile));
        Assert.That(((CopyInstallInstruction) installResult.Instructions[1]).Source, Is.EqualTo(win64Dll));
    }
}