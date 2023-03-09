using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bannerlord.LauncherManager;

public delegate void SetGameParametersDelegate(string executable, IReadOnlyList<string> gameParameters);
public delegate LoadOrder GetLoadOrderDelegate();
public delegate void SetLoadOrderDelegate(LoadOrder loadOrder);
public delegate void SendNotificationDelegate(string id, NotificationType type, string message, uint displayMs);
public delegate void SendDialogDelegate(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult);
public delegate string GetInstallPathDelegate();
public delegate byte[]? ReadFileContentDelegate(string filePath, int offset, int length);
public delegate void WriteFileContentDelegate(string filePath, byte[]? data);
public delegate string[]? ReadDirectoryFileListDelegate(string directoryPath);
public delegate string[]? ReadDirectoryListDelegate(string directoryPath);
public delegate IModuleViewModel[]? GetAllModuleViewModelsDelegate();
public delegate IModuleViewModel[]? GetModuleViewModelsDelegate();
public delegate void SetModuleViewModelsDelegate(IReadOnlyList<IModuleViewModel> moduleViewModels);
public delegate LauncherOptions GetOptionsDelegate();
public delegate LauncherState GetStateDelegate();