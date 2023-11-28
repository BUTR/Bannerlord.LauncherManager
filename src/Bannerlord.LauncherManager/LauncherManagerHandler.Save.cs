using Bannerlord.LauncherManager.Models;

using System;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual SaveMetadata[] GetSaveFiles() => Array.Empty<SaveMetadata>();

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual SaveMetadata? GetSaveMetadata(string fileName, ReadOnlySpan<byte> data) => null;

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual string? GetSaveFilePath(string saveFile) => null;
}