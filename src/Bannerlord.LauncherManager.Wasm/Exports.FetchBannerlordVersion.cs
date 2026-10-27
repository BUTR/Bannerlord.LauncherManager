using FetchBannerlordVersion;

using System;
using System.IO;
using System.Runtime.InteropServices.JavaScript;

namespace Bannerlord.LauncherManager.Wasm;

/// <summary>
/// FetchBannerlordVersion reads the game files from disk, and the runtime has no access to it.
/// JS reads the two files it looks at and they're staged into the runtime's in-memory file system as a game folder.
/// </summary>
public static partial class FetchBannerlordVersionExports
{
    [JSExport]
    public static int GetChangeSet(byte[]? libAssembly, byte[]? versionXml, string libAssemblyName)
    {
        using var game = StagedGameFolder.Create(libAssembly, versionXml, libAssemblyName);
        return Fetcher.GetChangeSet(game.Path, libAssemblyName);
    }

    [JSExport]
    public static string GetVersion(byte[]? libAssembly, byte[]? versionXml, string libAssemblyName)
    {
        using var game = StagedGameFolder.Create(libAssembly, versionXml, libAssemblyName);
        return Fetcher.GetVersion(game.Path, libAssemblyName);
    }

    [JSExport]
    public static int GetVersionType(byte[]? libAssembly, byte[]? versionXml, string libAssemblyName)
    {
        using var game = StagedGameFolder.Create(libAssembly, versionXml, libAssemblyName);
        return (int) Fetcher.GetVersionType(game.Path, libAssemblyName);
    }
}

internal readonly struct StagedGameFolder : IDisposable
{
    // Fetcher checks bin/Win64_Shipping_Client first, the Xbox folder only when it's missing
    private const string BinFolder = "Win64_Shipping_Client";

    private static int _counter;

    public string Path { get; }

    private StagedGameFolder(string path) => Path = path;

    public static StagedGameFolder Create(byte[]? libAssembly, byte[]? versionXml, string libAssemblyName)
    {
        var root = $"/staged-game-{_counter++}";
        var bin = System.IO.Path.Combine(root, "bin", BinFolder);
        Directory.CreateDirectory(bin);
        if (libAssembly is not null)
            File.WriteAllBytes(System.IO.Path.Combine(bin, System.IO.Path.GetFileName(libAssemblyName)), libAssembly);
        if (versionXml is not null)
            File.WriteAllBytes(System.IO.Path.Combine(bin, "Version.xml"), versionXml);
        return new StagedGameFolder(root);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, true);
        }
        catch (IOException) { }
    }
}
