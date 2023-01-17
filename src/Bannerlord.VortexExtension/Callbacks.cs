using System;
using Bannerlord.VortexExtension.Models;

namespace Bannerlord.VortexExtension
{
    public delegate Profile GetActiveProfileDelegate();
    public delegate Profile GetProfileByIdDelegate(ReadOnlySpan<char> profileId);
    public delegate ReadOnlySpan<char> GetActiveGameIdDelegate();
    public delegate void SetGameParametersDelegate( ReadOnlySpan<char> executable, string[] gameParameters);
    public delegate LoadOrder GetLoadOrderDelegate();
    public delegate void SetLoadOrderDelegate(LoadOrder loadOrder);
    public delegate ReadOnlySpan<char> TranslateStringDelegate(ReadOnlySpan<char> text);
    public delegate void SendNotificationDelegate(ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs);
    public delegate ReadOnlySpan<char> GetInstallPathDelegate();
    public delegate string? ReadFileContentDelegate(ReadOnlySpan<char> filePath);
    public delegate string[]? ReadDirectoryFileListDelegate(ReadOnlySpan<char> directoryPath);
}