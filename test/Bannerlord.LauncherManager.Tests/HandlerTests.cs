using Bannerlord.LauncherManager.Models;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Tests
{
    public class LauncherManagerHandlerExposer : LauncherManagerHandler
    {
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
        private const string VortexTestMod = "./Data/vortex/mountandblade2bannerlord/mods/Test/";

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
                fs.Read(data, 0, length);
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
                {"Test2", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, Index = 0 }},
                {"Test", new LoadOrderEntry { Id = "", Name = "", IsSelected = true, Index = 1 }},
            };
            var expectedLoadOrderIds = new[] { "Test", "Test2" };

            var moduleViewModels = Array.Empty<IModuleViewModel>();

            var handler = new LauncherManagerHandlerExposer();
            handler.RegisterCallbacks(
                loadLoadOrder: null!,
                saveLoadOrder: lo => loadOrder = lo,
                sendNotification: (id, type, message, ms) => { },
                sendDialog: null!,
                setGameParameters: (executable, parameters) => { },
                getInstallPath: () => Path.GetFullPath(GamePath)!,
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: Directory.GetFiles,
                readDirectoryList: Directory.GetDirectories,
                getModuleViewModels: () => moduleViewModels,
                setModuleViewModels: null!,
                getOptions: null!,
                getState: null!);

            var modules = handler.GetModules();
            moduleViewModels = new IModuleViewModel[]
            {
                new ModuleViewModel()
                {
                    ModuleInfoExtended = modules.First(x => x.Id == "Test"),
                    IsValid = true,
                    IsSelected = true,
                    IsDisabled = false,
                    Index = 0,
                },
                new ModuleViewModel()
                {
                    ModuleInfoExtended = modules.First(x => x.Id == "Test2"),
                    IsValid = true,
                    IsSelected = true,
                    IsDisabled = false,
                    Index = 1,
                },
            };

            handler.Sort();

            Assert.AreEqual(expectedLoadOrderIds, loadOrder.Select(x => x.Key).ToArray());
        }

        [Test]
        public void ModuleProvider_GetModules_Test()
        {
            var handler = new LauncherManagerHandlerExposer();
            handler.RegisterCallbacks(
                loadLoadOrder: null!,
                saveLoadOrder: null!,
                sendNotification: null!,
                sendDialog: null!,
                setGameParameters: null!,
                getInstallPath: () => Path.GetFullPath(GamePath),
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: s => Directory.GetDirectories(s),
                getModuleViewModels: null!,
                setModuleViewModels: null!,
                getOptions: null!,
                getState: null!);
            var modules = handler.GetModules().ToList();

            Assert.GreaterOrEqual(modules.Count, 1);
        }

        [Test]
        public void Handler_TestModule_tTest()
        {
            var files = new[]
            {
                "Test\\",
                "Test\\SubModule.xml",
            };

            var handler = new LauncherManagerHandlerExposer();
            handler.RegisterCallbacks(
                loadLoadOrder: null!,
                saveLoadOrder: null!,
                sendNotification: null!,
                sendDialog: null!,
                setGameParameters: null!,
                getInstallPath: null!,
                readFileContent: null!,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!,
                getOptions: null!,
                getState: null!);
            var installResult = handler.TestModuleContent(files);
            Assert.NotNull(installResult);
            Assert.IsTrue(installResult.Supported);
        }

        [Test]
        public void Handler_InstallModule_Test()
        {
            var files = new[]
            {
                "Test\\",
                "Test\\SubModule.xml",
            };

            var handler = new LauncherManagerHandlerExposer();
            handler.RegisterCallbacks(
                loadLoadOrder: null!,
                saveLoadOrder: null!,
                sendNotification: null!,
                sendDialog: null!,
                setGameParameters: null!,
                getInstallPath: null!,
                readFileContent: Read,
                writeFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: null!,
                getModuleViewModels: null!,
                setModuleViewModels: null!,
                getOptions: null!,
                getState: null!);
            var installResult = handler.InstallModuleContent(files, Path.GetFullPath(VortexTestMod));
            Assert.NotNull(installResult);
            Assert.NotNull(installResult.Instructions);
            Assert.AreEqual(2, installResult.Instructions.Count);
            Assert.AreEqual(InstallInstructionType.Copy, installResult.Instructions[0].Type);
            Assert.AreEqual(InstallInstructionType.ModuleInfo, installResult.Instructions[1].Type);
        }
    }
}