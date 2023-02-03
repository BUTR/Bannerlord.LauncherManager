using Bannerlord.LauncherManager.Models;

using NUnit.Framework;

using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Tests
{
    public class HandlerTests
    {
        private const string GamePath = "./Data/game/";
        private const string VortexTestMod = "./Data/vortex/mountandblade2bannerlord/mods/Test/";

        [Test]
        public void Sorter_Sort_Test()
        {
            var loadOrder = new LoadOrder
            {
                {"Test2", new LoadOrderEntry { Enabled = true, Pos = 1 }},
                {"Test", new LoadOrderEntry { Enabled = true, Pos = 2 }},
            };
            var expectedLoadOrderIds = new[] { "Test", "Test2" };

            var handler = new VortexExtensionHandler();
            handler.RegisterCallbacks(
                getActiveProfile: () => new Profile { GameId = Constants.GameID, Id = "Profile" },
                getLoadOrder: () => loadOrder,
                setLoadOrder: lo => loadOrder = lo,
                sendNotification: (id, type, message, ms) => { },
                translateString: (text) => text,
                setGameParameters: (executable, parameters) => { },
                getActiveGameId: null!,
                getProfileById: null!,
                getInstallPath: () => Path.GetFullPath(GamePath),
                readFileContent: path => File.ReadAllText(path.ToString()),
                readDirectoryFileList: path => Directory.GetFiles(path.ToString()),
                readDirectoryList: path => Directory.GetDirectories(path.ToString()));

            handler.Sort();

            Assert.AreEqual(expectedLoadOrderIds, loadOrder.OrderBy(x => x.Value.Pos).Select(x => x.Key).ToArray());
        }

        [Test]
        public void ModuleProvider_GetModules_Test()
        {
            var handler = new VortexExtensionHandler();
            handler.RegisterCallbacks(
                getActiveProfile: null!,
                getLoadOrder: null!,
                setLoadOrder: null!,
                sendNotification: null!,
                translateString: null!,
                setGameParameters: null!,
                getActiveGameId: null!,
                getProfileById: null!,
                getInstallPath: () => Path.GetFullPath(GamePath),
                readFileContent: path => File.ReadAllText(path.ToString()),
                readDirectoryFileList: path => Directory.GetFiles(path.ToString()),
                readDirectoryList: path => Directory.GetDirectories(path.ToString()));
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

            var handler = new VortexExtensionHandler();
            handler.RegisterCallbacks(
                getActiveProfile: null!,
                getLoadOrder: null!,
                setLoadOrder: null!,
                sendNotification: null!,
                translateString: null!,
                setGameParameters: null!,
                getActiveGameId: null!,
                getProfileById: null!,
                getInstallPath: () => Path.GetFullPath(GamePath),
                readFileContent: null!,
                readDirectoryFileList: null!,
                readDirectoryList: null!);
            var installResult = handler.TestModuleContent(files, Constants.GameID);
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

            var handler = new VortexExtensionHandler();
            handler.RegisterCallbacks(
                getActiveProfile: null!,
                getLoadOrder: null!,
                setLoadOrder: null!,
                sendNotification: null!,
                translateString: null!,
                setGameParameters: null!,
                getActiveGameId: null!,
                getProfileById: null!,
                getInstallPath: () => Path.GetFullPath(GamePath),
                readFileContent: path => File.ReadAllText(path.ToString()),
                readDirectoryFileList: path => Directory.GetFiles(path.ToString()),
                readDirectoryList: path => Directory.GetDirectories(path.ToString()));
            var installResult = handler.InstallModuleContent(files, Path.GetFullPath(VortexTestMod));
            Assert.NotNull(installResult);
            Assert.NotNull(installResult.Instructions);
            Assert.AreEqual(2, installResult.Instructions.Count);
            Assert.AreEqual(InstructionType.copy, installResult.Instructions[0].Type);
            Assert.AreEqual(InstructionType.attribute, installResult.Instructions[1].Type);
        }
    }
}