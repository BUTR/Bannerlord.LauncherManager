using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Bannerlord.LauncherManager.Native;

/// <summary>
/// Module lists kept for the JS side's ModuleValidator: its list is parsed once instead of on every call. A call that
/// takes a module list takes its JSON, parsed for that call, or "#" and the handle of a kept list.
/// Shared by the native library and the WebAssembly runtime
/// </summary>
public static class ModuleLists
{
    private static readonly Dictionary<int, ModuleInfoExtended[]> _lists = new();
    private static int _nextHandle = 1;

    public static int Register(ModuleInfoExtended[] modules)
    {
        lock (_lists)
        {
            var handle = _nextHandle++;
            _lists[handle] = modules;
            return handle;
        }
    }

    public static void Release(int handle)
    {
        lock (_lists)
            _lists.Remove(handle);
    }

    /// <returns>The kept list of a "#&lt;handle&gt;" argument, null for the JSON of a list</returns>
    public static ModuleInfoExtended[]? FromArgument(ReadOnlySpan<char> argument)
    {
        if (argument.Length == 0 || argument[0] != '#')
            return null;

        return Get(int.Parse(argument.Slice(1), NumberStyles.None, CultureInfo.InvariantCulture));
    }

    /// <returns>The kept list of a "#&lt;handle&gt;" argument in UTF-8, null for the JSON of a list</returns>
    public static ModuleInfoExtended[]? FromArgument(ReadOnlySpan<byte> argument)
    {
        if (argument.Length == 0 || argument[0] != (byte) '#')
            return null;

        return Get(int.Parse(argument.Slice(1), NumberStyles.None, CultureInfo.InvariantCulture));
    }

    private static ModuleInfoExtended[] Get(int handle)
    {
        lock (_lists)
            return _lists.TryGetValue(handle, out var modules) ? modules : throw new ArgumentException($"No module list #{handle}, its ModuleValidator was disposed");
    }

    /// <summary>
    /// The modules' positions in the list they came from, as a JSON array of numbers: the JS side picks them from the
    /// list it sent instead of parsing copies, a sorted 300 module list is 750K chars of JSON. The sorter and the
    /// dependency lookups only return modules of the list they get
    /// </summary>
    /// <returns>Null when a module isn't one of the source's, the caller returns the modules then</returns>
    public static string? Positions(IReadOnlyList<ModuleInfoExtended> source, IReadOnlyList<ModuleInfoExtended> result)
    {
        var positions = new Dictionary<ModuleInfoExtended, int>(source.Count, ReferenceEqualityComparer.Instance);
        for (var i = 0; i < source.Count; i++)
            positions.TryAdd(source[i], i);

        var json = new StringBuilder(result.Count * 4 + 2).Append('[');
        for (var i = 0; i < result.Count; i++)
        {
            if (!positions.TryGetValue(result[i], out var position))
                return null;
            if (i > 0) json.Append(',');
            json.Append(position.ToString(CultureInfo.InvariantCulture));
        }
        return json.Append(']').ToString();
    }
}
