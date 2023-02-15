﻿using Bannerlord.BUTR.Shared.Extensions;
using Bannerlord.BUTRLoader.Helpers;
using Bannerlord.BUTRLoader.LauncherEx;
using Bannerlord.LauncherManager;
using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using Ookii.Dialogs.WinForms;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using TaleWorlds.MountAndBlade.Launcher.Library;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;

namespace Bannerlord.BUTRLoader
{
    internal partial class BUTRLauncherManagerHandler : LauncherManagerHandler
    {
        private static readonly AccessTools.FieldRef<StandaloneUIDomain>? _standaloneUIDomain =
            AccessTools2.StaticFieldRefAccess<StandaloneUIDomain>("TaleWorlds.MountAndBlade.Launcher.Library.Program:_standaloneUIDomain");

        private static readonly Harmony _harmony = new("bannerlord.butrloader.launchermanager");
        public static readonly BUTRLauncherManagerHandler Default = new();

        public new Dictionary<string, ModuleInfoExtended> ExtendedModuleInfoCache => base.ExtendedModuleInfoCache;


        private string _executable = Constants.BannerlordExecutable;
        private string _executableParameters = string.Empty;

        private Func<LauncherState>? _getState;
        private Func<IEnumerable<IModuleViewModel>>? _getModuleViewModels;
        private Action<IEnumerable<IModuleViewModel>>? _setModuleViewModels;

        private readonly UserDataManager _userDataManager = _standaloneUIDomain!().UserDataManager;
        private readonly string _installPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../", "../"));

        private BUTRLauncherManagerHandler()
        {
            _harmony.Patch(
                AccessTools2.DeclaredPropertyGetter(typeof(LauncherUI), "AdditionalArgs"),
                postfix: new HarmonyMethod(AccessTools2.DeclaredMethod(typeof(BUTRLauncherManagerHandler), nameof(AdditionalArgsPostfix)), priority: 10000));

            var ids = new ConcurrentDictionary<string, object?>();
            RegisterCallbacks(
                loadLoadOrder: LoadTWLoadOrder,
                saveLoadOrder: loadOrder =>
                {
                    var userGameTypeData = _userDataManager.UserData.SingleplayerData;
                    userGameTypeData.ModDatas.Clear();
                    foreach (var (id, entry) in loadOrder)
                    {
                        userGameTypeData.ModDatas.Add(new UserModData
                        {
                            Id = id,
                            IsSelected = entry.IsSelected,
                        });
                    }
                    _userDataManager.SaveUserData();
                },
                sendNotification: (id, type, message, ms) =>
                {
                    if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();

                    // Prevents message spam
                    if (ids.TryAdd(id, null)) return;
                    using var cts = new CancellationTokenSource();
                    _ = Task.Delay(TimeSpan.FromMilliseconds(ms), cts.Token).ContinueWith(x => ids.TryRemove(id, out _));

                    var translatedMessage = new BUTRTextObject(message).ToString();
                    switch (type)
                    {
                        case NotificationType.Hint:
                        {
                            HintManager.ShowHint(translatedMessage);
                            cts.Cancel();
                            break;
                        }
                        case NotificationType.Info:
                        {
                            // TODO:
                            HintManager.ShowHint(translatedMessage);
                            cts.Cancel();
                            break;
                        }
                        default:
                            MessageBox.Show(translatedMessage);
                            cts.Cancel();
                            break;
                    }
                },
                sendDialog: (type, title, message, filters, onResult) =>
                {
                    switch (type)
                    {
                        case DialogType.Warning:
                        {
                            var split = message.Split(new[] { "--CONTENT-SPLIT--" }, StringSplitOptions.RemoveEmptyEntries);
                            using var okButton = new TaskDialogButton(ButtonType.Yes);
                            using var cancelButton = new TaskDialogButton(ButtonType.No);
                            using var dialog = new TaskDialog
                            {
                                MainIcon = TaskDialogIcon.Warning,
                                WindowTitle = new BUTRTextObject(title).ToString(),
                                MainInstruction = split[0],
                                Content = split.Length > 1 ? split[1] : string.Empty,
                                Buttons = { okButton, cancelButton },
                                CenterParent = true,
                                AllowDialogCancellation = true,
                            };
                            onResult(dialog.ShowDialog() == okButton ? "true" : "false");
                            return;
                        }
                        case DialogType.FileOpen:
                        {
                            var filter = string.Join("|", filters.Select(x => $"{x.Name} ({string.Join(", ", x.Extensions)}|{string.Join(", ", x.Extensions)}"));
                            var dialog = new VistaOpenFileDialog
                            {
                                Title = title,
                                Filter = filter,

                                CheckFileExists = true,
                                CheckPathExists = true,
                                ReadOnlyChecked = true,
                                Multiselect = false,
                                ValidateNames = true,
                            };
                            onResult(dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty);
                            return;
                        }
                        case DialogType.FileSave:
                        {
                            var fileName = message;
                            var filter = string.Join("|", filters.Select(x => $"{x.Name} ({string.Join(", ", x.Extensions)}|{string.Join(", ", x.Extensions)}"));
                            var dialog = new VistaSaveFileDialog
                            {
                                Title = title,
                                Filter = filter,
                                FileName = fileName,

                                CheckFileExists = false,
                                CheckPathExists = false,

                                ValidateNames = true,
                            };
                            onResult(dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty);
                            return;
                        }
                    }
                },
                setGameParameters: (executable, parameters) =>
                {
                    _executable = executable;
                    _executableParameters = string.Join(" ", parameters);
                },
                getInstallPath: () => _installPath,
                readFileContent: (path, offset, length) =>
                {
                    if (!File.Exists(path)) return null;

                    if (offset == 0 && length == -1)
                    {
                        return File.ReadAllBytes(path);
                    }
                    else if (offset >= 0 && length > 0)
                    {
                        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var data = new byte[length];
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Read(data, 0, length);
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                },
                writeFileContent: File.WriteAllBytes,
                readDirectoryFileList: Directory.GetFiles,
                readDirectoryList: Directory.GetDirectories,
                getModuleViewModels: () => _getModuleViewModels?.Invoke()?.ToArray() ?? Array.Empty<IModuleViewModel>(),
                setModuleViewModels: (orderedViewModels) => _setModuleViewModels?.Invoke(orderedViewModels),
                getOptions: GetTWOptions,
                getState: () => _getState?.Invoke() ?? LauncherState.Empty
            );
        }
        private static void AdditionalArgsPostfix(ref string __result)
        {
            __result = Default._executableParameters;
        }

        public void RegisterStateProvider(Func<LauncherState> getState)
        {
            _getState = getState;
        }

        public void RegisterModuleViewModelProvider(Func<IEnumerable<IModuleViewModel>> getModuleViewModels, Action<IEnumerable<IModuleViewModel>> setModuleViewModels)
        {
            _getModuleViewModels = getModuleViewModels;
            _setModuleViewModels = setModuleViewModels;
        }

        public LoadOrder LoadTWLoadOrder()
        {
            var state = _getState?.Invoke() ?? LauncherState.Empty;

            var userGameTypeData = state.IsSingleplayer ? _userDataManager.UserData.SingleplayerData : _userDataManager.UserData.MultiplayerData;
            return new LoadOrder(userGameTypeData.ModDatas.Select((x, i) => new LoadOrderEntry
            {
                Id = x.Id,
                Name = "",
                IsSelected = x.IsSelected,
                Index = i,
            }));
        }

        public LauncherOptions GetTWOptions() => new()
        {
            BetaSorting = LauncherSettings.BetaSorting,
            FixCommonIssues = LauncherSettings.FixCommonIssues,
            UnblockFiles = LauncherSettings.UnblockFiles,
            Language = Manager.GetActiveLanguage(),
        };

        public new bool TryOrderByLoadOrderTW(IEnumerable<string> loadOrder, Func<string, bool> isModuleSelected, [NotNullWhen(false)] out IReadOnlyList<string>? issues,
            [NotNullWhen(true)] out IReadOnlyList<IModuleViewModel>? orderedModules, bool overwriteWhenFailure = false)
            => base.TryOrderByLoadOrderTW(loadOrder, isModuleSelected, out issues, out orderedModules, overwriteWhenFailure);
    }
}