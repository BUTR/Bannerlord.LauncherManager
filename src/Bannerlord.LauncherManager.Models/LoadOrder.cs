using Bannerlord.ModuleManager;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record LoadOrderEntry
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool IsSelected { get; set; }
    public required int Index { get; set; }

    public LoadOrderEntry() { }
    [SetsRequiredMembers, JsonConstructor]
    public LoadOrderEntry(string id, string name, bool isSelected, int index) => (Id, Name, IsSelected, Index) = (id, name, isSelected, index);
    [SetsRequiredMembers]
    public LoadOrderEntry(ModuleInfoExtended moduleInfoExtended, bool isSelected, int index) => (Id, Name, IsSelected, Index) = (moduleInfoExtended.Id, moduleInfoExtended.Name, isSelected, index);
    [SetsRequiredMembers]
    public LoadOrderEntry(IModuleViewModel moduleViewModel) => (Id, Name, IsSelected, Index) = (moduleViewModel.ModuleInfoExtended.Id, moduleViewModel.ModuleInfoExtended.Name, moduleViewModel.IsSelected, moduleViewModel.Index);
}

public record LoadOrder : IDictionary<string, LoadOrderEntry>
{
    private readonly IDictionary<string, LoadOrderEntry> _implementation = new Dictionary<string, LoadOrderEntry>();

    public int Count => _implementation.Count;
    public bool IsReadOnly => _implementation.IsReadOnly;

    public LoadOrder() { }
    public LoadOrder(IEnumerable<LoadOrderEntry> enumerable) => _implementation = enumerable.ToDictionary(x => x.Id, x => x);
    public LoadOrder(IEnumerable<ModuleInfoExtended> enumerable) => _implementation = enumerable.Select((x, i) => (x, i)).ToDictionary(x => x.x.Id, x => new LoadOrderEntry(x.x, true, x.i));
    public LoadOrder(IEnumerable<IModuleViewModel> enumerable) => _implementation = enumerable.ToDictionary(x => x.ModuleInfoExtended.Id, x => new LoadOrderEntry(x));

    public IEnumerator<KeyValuePair<string, LoadOrderEntry>> GetEnumerator() => _implementation.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _implementation).GetEnumerator();

    public void Add(KeyValuePair<string, LoadOrderEntry> item) => _implementation.Add(item);
    public void Clear() => _implementation.Clear();
    public bool Contains(KeyValuePair<string, LoadOrderEntry> item) => _implementation.Contains(item);
    public void CopyTo(KeyValuePair<string, LoadOrderEntry>[] array, int arrayIndex) => _implementation.CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<string, LoadOrderEntry> item) => _implementation.Remove(item);
    public void Add(string key, LoadOrderEntry value) => _implementation.Add(key, value);
    public bool ContainsKey(string key) => _implementation.ContainsKey(key);
    public bool Remove(string key) => _implementation.Remove(key);
    public bool TryGetValue(string key, out LoadOrderEntry value) => _implementation.TryGetValue(key, out value);
    public LoadOrderEntry this[string key] { get => _implementation[key]; set => _implementation[key] = value; }
    public ICollection<string> Keys => _implementation.Keys;
    public ICollection<LoadOrderEntry> Values => _implementation.Values;
}