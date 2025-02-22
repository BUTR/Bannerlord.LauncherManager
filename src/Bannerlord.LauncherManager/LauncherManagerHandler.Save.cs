using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual Task<IReadOnlyList<SaveMetadata>> GetSaveFilesAsync() => Task.FromResult<IReadOnlyList<SaveMetadata>>([]);

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual Task<SaveMetadata?> GetSaveMetadataAsync(string fileName, ReadOnlyMemory<byte> data) => Task.FromResult<SaveMetadata?>(null);

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual Task<string?> GetSaveFilePathAsync(string saveFile) => Task.FromResult<string?>(null);
}