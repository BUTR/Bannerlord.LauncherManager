#if !NETSTANDARD2_1_OR_GREATER
using Bannerlord.LauncherManager.Extensions;
#endif
using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using ApplicationVersion = Bannerlord.ModuleManager.ApplicationVersion;

namespace Bannerlord.LauncherManager.Utils;

public class ModuleListHandler
{
    private record ModuleListEntry(string Id, ApplicationVersion Version, string? Url = null);
    private record ModuleMismatch(string Id, ApplicationVersion OriginalVersion, ApplicationVersion CurrentVersion)
    {
        public override string ToString() => new BUTRTextObject("{=nYVWoomO}{MODULEID}. Required {REQUIREDVERSION}. Installed {ACTUALVERSION}")
            .SetTextVariable("MODULEID", Id)
            .SetTextVariable("REQUIREDVERSION", OriginalVersion.ToString())
            .SetTextVariable("ACTUALVERSION", CurrentVersion.ToString())
            .ToString();
    }

    private readonly LauncherManagerHandler _launcherManager;

    public ModuleListHandler(LauncherManagerHandler launcherManager)
    {
        _launcherManager = launcherManager;
    }

    private void ShowMissingWarning(IEnumerable<string> missingModuleIds, Action<bool> onResult)
    {
        var missing = new BUTRTextObject("{=GtDRbC3m}Missing Modules:{NL}{MODULES}")
            .SetTextVariable("MODULES", "").ToString();
        _launcherManager.ShowWarning(
            new BUTRTextObject("Import Warning").ToString(),
            missing,
            $"{string.Join("\n", missingModuleIds)}{Environment.NewLine}{Environment.NewLine}{new BUTRTextObject("{=hgew15HH}Continue import?")}",
            onResult
        );
    }
    private void ShowVersionWarning(IEnumerable<string> mismatchedVersions, Action<bool> onResult)
    {
        var mismatched = new BUTRTextObject("{=BuMom4Jt}Mismatched Module Versions:{NL}{MODULEVERSIONS}")
            .SetTextVariable("NL", Environment.NewLine)
            .SetTextVariable("MODULEVERSIONS", string.Join(Environment.NewLine, mismatchedVersions)).ToString();
        var split = mismatched.Split('\n');
        _launcherManager.ShowWarning(
            new BUTRTextObject("Import Warning").ToString(),
            split[0],
            $"{split[1]}{Environment.NewLine}{Environment.NewLine}{new BUTRTextObject("{=hgew15HH}Continue import?")}",
            onResult
        );
    }
    private void ReadImportList(byte[] data, Action<ModuleInfoExtendedWithPath[]> onResult)
    {
        static IEnumerable<string> ReadAllLines(TextReader reader)
        {
            while (reader.ReadLine() is { } line)
            {
                yield return line;
            }
        }
        IEnumerable<ModuleListEntry> Deserialize(IEnumerable<string> content)
        {
            var list = new List<ModuleListEntry>();
            foreach (var line in content)
            {
                if (line.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries) is { Length: 2 } split)
                {
                    var version = ApplicationVersion.TryParse(split[1], out var versionVar) ? versionVar : ApplicationVersion.Empty;
                    list.Add(new ModuleListEntry(split[0], version));
                }
            }
            var nativeChangeset = list.Find(x => x.Id == Constants.NativeModule)?.Version.ChangeSet is var y and not 0 ? y : _launcherManager.GetChangeset();
            foreach (var entry in list)
            {
                var version = entry.Version;
                yield return version.ChangeSet == nativeChangeset
                    ? entry with { Version = new ApplicationVersion(version.ApplicationVersionType, version.Major, version.Minor, version.Revision, 0) }
                    : entry;
            }
        }

        var moduleViewModels = _launcherManager.GetModuleViewModels() ?? Array.Empty<IModuleViewModel>();

        var idDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (idDuplicates.Count > 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=izSm5f85}Duplicate Module Ids:{NL}{MODULEIDS}").SetTextVariable("MODULEIDS", string.Join("\n", idDuplicates))}");
            onResult(Array.Empty<ModuleInfoExtendedWithPath>());
            return;
        }

        using var stream = new MemoryStream(data);
        using var reader = new StreamReader(stream);
        var importedModules = Deserialize(ReadAllLines(reader)).ToArray();

        void ImportListInternal()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (id, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Id == id) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(id, version, launcherModuleVersion));
            }


            ModuleInfoExtendedWithPath[] GetResult() => importedModules.Select(x => moduleViewModels.First(y => y.ModuleInfoExtended.Id == x.Id)).Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                ShowVersionWarning(mismatchedVersions.Select(x => x.ToString()), result =>
                {
                    if (!result)
                    {
                        onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                        return;
                    }

                    onResult(GetResult());
                });
                return;
            }
            onResult(GetResult());
        }

        var importedModuleIds = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleIds = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).ToHashSet();
        var missingModuleIds = importedModuleIds.Except(currentModuleIds).ToList();
        if (missingModuleIds.Count > 0)
        {
            ShowMissingWarning(missingModuleIds, result =>
            {
                if (!result)
                {
                    onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                    return;
                }

                ImportListInternal();
            });
            return;
        }

        ImportListInternal();
    }
    private void ReadSaveFile(byte[] data, Action<ModuleInfoExtendedWithPath[]> onResult)
    {
        var moduleViewModels = _launcherManager.GetModuleViewModels() ?? Array.Empty<IModuleViewModel>();

        var nameDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended.Name).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (nameDuplicates.Count > 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=vCwH9226}Duplicate Module Names:{NL}{MODULENAMES}").SetTextVariable("MODULENAMES", string.Join("\n", nameDuplicates))}");
            onResult(Array.Empty<ModuleInfoExtendedWithPath>());
            return;
        }

        if (_launcherManager.GetSaveMetadata(string.Empty, data) is not { } metadata)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=epU06HID}Failed to read the save file!")}");
            onResult(Array.Empty<ModuleInfoExtendedWithPath>());
            return;
        }

        var changeset = metadata.GetChangeSet();
        var importedModules = metadata.GetModules().Select(x =>
        {
            var version = metadata.GetModuleVersion(x);
            if (version.ChangeSet == changeset)
                version = new ApplicationVersion(version.ApplicationVersionType, version.Major, version.Minor, version.Revision, 0);
            return new ModuleListEntry(x, version);
        }).ToArray();

        void ImportSaveInternal()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (name, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Name == name) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(name, version, launcherModuleVersion));
            }


            ModuleInfoExtendedWithPath[] GetResult() => importedModules.Select(x => moduleViewModels.First(y => y.ModuleInfoExtended.Name == x.Id)).Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                ShowVersionWarning(mismatchedVersions.Select(x => x.ToString()), result =>
                {
                    if (!result)
                    {
                        onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                        return;
                    }

                    onResult(GetResult());
                });
                return;
            }
            onResult(GetResult());
        }

        var importedModuleNames = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleNames = moduleViewModels.Select(x => x.ModuleInfoExtended.Name).ToHashSet();
        var missingModuleNames = importedModuleNames.Except(currentModuleNames).ToList();
        if (missingModuleNames.Count > 0)
        {
            ShowMissingWarning(missingModuleNames.Select(x => x.ToString()), result =>
            {
                if (!result)
                {
                    onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                    return;
                }

                ImportSaveInternal();
            });
            return;
        }

        ImportSaveInternal();
    }
    private void ReadNovusPreset(byte[] data, Action<ModuleInfoExtendedWithPath[]> onResult)
    {
        var moduleViewModels = _launcherManager.GetModuleViewModels() ?? Array.Empty<IModuleViewModel>();

        var idDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (idDuplicates.Count > 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=izSm5f85}Duplicate Module Ids:{NL}{MODULEIDS}").SetTextVariable("MODULEIDS", string.Join("\n", idDuplicates))}");
            onResult(Array.Empty<ModuleInfoExtendedWithPath>());
            return;
        }

        var document = new XmlDocument();
        using var stream = new MemoryStream(data);
        document.Load(stream);

        var importedModules = new List<ModuleListEntry>();
        foreach (var xmlNode in document.DocumentElement?.SelectNodes("PresetModule")?.OfType<XmlNode>() ?? Enumerable.Empty<XmlNode>())
        {
            if (xmlNode.NodeType == XmlNodeType.Comment)
                continue;

            if (xmlNode.Attributes?["Id"] is null)
                continue;

            var id = xmlNode.Attributes["Id"].InnerText;
            var version = xmlNode.Attributes["RequiredVersion"].InnerText;
            if (!string.IsNullOrEmpty(version) && char.IsNumber(version[0]))
                version = $"v{version}";
            importedModules.Add(new ModuleListEntry(id, ApplicationVersion.TryParse(version, out var versionVar) ? versionVar : ApplicationVersion.Empty));

        }

        void ImportNovusInternal()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (id, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Id == id) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(id, version, launcherModuleVersion));
            }

            ModuleInfoExtendedWithPath[] GetResult() => importedModules.Select(x => moduleViewModels.FirstOrDefault(y => y.ModuleInfoExtended.Id == x.Id)).OfType<IModuleViewModel>().Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                ShowVersionWarning(mismatchedVersions.Select(x => x.ToString()), result =>
                {
                    if (!result)
                    {
                        onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                        return;
                    }

                    onResult(GetResult());
                });
                return;
            }
            onResult(GetResult());
        }

        var importedModuleIds = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleIds = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).ToHashSet();
        var missingModuleIds = importedModuleIds.Except(currentModuleIds).ToList();
        if (missingModuleIds.Count > 0)
        {
            ShowMissingWarning(missingModuleIds.Select(x => x.ToString()), result =>
            {
                if (!result)
                {
                    onResult(Array.Empty<ModuleInfoExtendedWithPath>());
                    return;
                }

                ImportNovusInternal();
            });
            return;
        }

        ImportNovusInternal();
    }
    /// <summary>
    /// External<br/>
    /// </summary>
    public void Import(Action<bool> onResult)
    {
        _launcherManager.ShowFileOpen(
            new BUTRTextObject("{=DKRNkst2}Open a File with a Load Order").ToString(),
            new[]
            {
                    new DialogFileFilter("Bannerlord Module List", new []{ "*.bmlist" }),
                    new DialogFileFilter("Bannerlord Save File", new []{ "*.sav" }),
                    new DialogFileFilter("Novus Preset", new []{ "*.xml" }),
                    new DialogFileFilter("All files", new []{ "*.*" }),
            },
            fileName =>
            {
                if (string.IsNullOrEmpty(fileName) || _launcherManager.ReadFileContent(fileName, 0, -1) is not { } data) return;

                try
                {
                    switch (Path.GetExtension(fileName))
                    {
                        case ".bmlist":
                            ReadImportList(data, result =>
                        {
                            ImportInternal(result);
                            onResult(true);
                        }); break;
                        case ".sav":
                            ReadSaveFile(data, result =>
                        {
                            ImportInternal(result);
                            onResult(true);
                        }); break;
                        case ".xml":
                            ReadNovusPreset(data, result =>
                        {
                            ImportInternal(result);
                            onResult(true);
                        }); break;
                    }
                }
                catch (Exception e)
                {
                    _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\nException:\n{e}");
                    onResult(false);
                }
            }
        );
    }
    /// <summary>
    /// External<br/>
    /// </summary>
    public void ImportSaveFile(string saveFile, Action<bool> onResult)
    {
        if (_launcherManager.GetSaveFilePath(saveFile) is not { } saveFilePath)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            onResult(false);
            return;
        }

        if (_launcherManager.ReadFileContent(saveFilePath, 0, -1) is not { } data)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            onResult(false);
            return;
        }

        try
        {
            ReadSaveFile(data, result =>
            {
                ImportInternal(result);
                onResult(true);
            });
        }
        catch (Exception e)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\nException:\n{e}");
            onResult(false);
        }
    }
    private void ImportInternal(ModuleInfoExtendedWithPath[] modules)
    {
        if (modules.Length == 0)
            return;

        var loadOrderValidationIssues = LoadOrderChecker.IsLoadOrderCorrect(modules).ToList();
        if (loadOrderValidationIssues.Count != 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=HvvA78sZ}Load Order Issues:{NL}{LOADORDERISSUES}").SetTextVariable("LOADORDERISSUES", string.Join("\n", loadOrderValidationIssues))}");
            return;
        }

        var moduleIds = modules.Select(x => x.Id).ToHashSet();
        if (!_launcherManager.TryOrderByLoadOrder(modules.Select(x => x.Id), x => moduleIds.Contains(x), out var orderIssues, out var orderedModules))
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=HvvA78sZ}Load Order Issues:{NL}{LOADORDERISSUES}").SetTextVariable("LOADORDERISSUES", string.Join("\n", orderIssues))}");
            return;
        }
        _launcherManager.SetModuleViewModels(orderedModules);
    }

    private ModuleListEntry[] ReadSaveFileModuleList(byte[] data)
    {
        var moduleViewModels = _launcherManager.GetModuleViewModels() ?? Array.Empty<IModuleViewModel>();

        var nameDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended.Name).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (nameDuplicates.Count > 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=vCwH9226}Duplicate Module Names:{NL}{MODULENAMES}").SetTextVariable("MODULENAMES", string.Join("\n", nameDuplicates))}");
            return Array.Empty<ModuleListEntry>();
        }

        if (_launcherManager.GetSaveMetadata(string.Empty, data) is not { } metadata)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=epU06HID}Failed to read the save file!")}");
            return Array.Empty<ModuleListEntry>();
        }

        var changeset = metadata.GetChangeSet();
        var importedModules = metadata.GetModules().Select(x =>
        {
            var version = metadata.GetModuleVersion(x);
            if (version.ChangeSet == changeset)
                version = new ApplicationVersion(version.ApplicationVersionType, version.Major, version.Minor, version.Revision, 0);
            return new ModuleListEntry(x, version);
        }).ToArray();

        var importedModuleNames = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleNames = moduleViewModels.Select(x => x.ModuleInfoExtended.Name).ToHashSet();
        var missingModuleNames = importedModuleNames.Except(currentModuleNames).ToList();
        if (missingModuleNames.Count > 0)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=GtDRbC3m}Missing Modules:{NL}{MODULES}").SetTextVariable("MODULES", string.Join("\n", missingModuleNames))}");
            return Array.Empty<ModuleListEntry>();
        }

        return importedModules
            .Select(x => moduleViewModels.First(y => y.ModuleInfoExtended.Name == x.Id))
            .Select(x => x.ModuleInfoExtended)
            .Select(x => new ModuleListEntry(x.Id, x.Version, x.Url))
            .ToArray();
    }
    private static void SaveBMList(Stream stream, IEnumerable<ModuleListEntry> modules)
    {
        static string Serialize(IEnumerable<ModuleListEntry> modules)
        {
            var sb = new StringBuilder();
            foreach (var (id, version, _) in modules)
            {
                sb.AppendLine($"{id}: {version}");
            }
            return sb.ToString();
        }

        using var writer = new StreamWriter(stream);
        var content = Serialize(modules.Select(x => new ModuleListEntry(x.Id, x.Version)).ToArray());
        writer.Write(content);
    }
    private static void SaveNovusPreset(Stream stream, IEnumerable<ModuleListEntry> modules)
    {
        var document = new XmlDocument();

        var root = document.CreateElement("Preset");
        var nameAttribute = document.CreateAttribute("Name");
        nameAttribute.Value = "BUTRLoader Exported Load Order";
        var createdByAttribute = document.CreateAttribute("CreatedBy");
        createdByAttribute.Value = "BUTRLoader";
        var lastUpdatedAttribute = document.CreateAttribute("LastUpdated");
        lastUpdatedAttribute.Value = DateTime.Now.ToString("dd/MM/yyyy");
        root.Attributes.Append(nameAttribute);
        root.Attributes.Append(createdByAttribute);
        root.Attributes.Append(lastUpdatedAttribute);
        foreach (var module in modules)
        {
            var entryNode = document.CreateElement("PresetModule");

            var idAttribute = document.CreateAttribute("Id");
            idAttribute.Value = module.Id;
            var versionAttribute = document.CreateAttribute("RequiredVersion");
            versionAttribute.Value = module.Version.ToString();
            var urlAttribute = document.CreateAttribute("URL");
            urlAttribute.Value = module.Url;

            entryNode.Attributes.Append(idAttribute);
            entryNode.Attributes.Append(versionAttribute);
            entryNode.Attributes.Append(urlAttribute);

            root.AppendChild(entryNode);
        }
        document.AppendChild(root);

        using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        });
        document.Save(writer);
    }
    /// <summary>
    /// External<br/>
    /// </summary>
    public void Export()
    {
        _launcherManager.ShowFileSave(
            new BUTRTextObject("{=XSxlKweM}Save a Bannerlord Module List File").ToString(),
            "MyList.bmlist",
            new[]
            {
                    new DialogFileFilter("Bannerlord Module List", new []{ "*.bmlist" }),
                    new DialogFileFilter("Novus Preset", new []{ "*.xml" }),
            },
            fileName =>
            {
                if (string.IsNullOrEmpty(fileName)) return;

                try
                {
                    var moduleViewModels = _launcherManager.GetModuleViewModels() ?? Array.Empty<IModuleViewModel>();

                    var modules = moduleViewModels
                        .Where(x => x.IsSelected)
                        .Select(x => x.ModuleInfoExtended)
                        .Select(x => new ModuleListEntry(x.Id, x.Version, x.Url));

                    using var fs = new MemoryStream();
                    switch (Path.GetExtension(fileName))
                    {
                        case ".bmlist":
                            SaveBMList(fs, modules);
                            break;
                        case ".xml":
                            SaveNovusPreset(fs, modules);
                            break;
                    }
                    _launcherManager.WriteFileContent(fileName, fs.ToArray());
                }
                catch (Exception e)
                {
                    _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\nException:\n{e}");
                }
                _launcherManager.ShowHint(new BUTRTextObject("{=VwFQTk5z}Successfully exported list!"));
            }
        );
    }
    /// <summary>
    /// External<br/>
    /// </summary>
    public void ExportSaveFile(string saveFile)
    {
        if (_launcherManager.GetSaveFilePath(saveFile) is not { } saveFilePath)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return;
        }

        if (_launcherManager.ReadFileContent(saveFilePath, 0, -1) is not { } data)
        {
            _launcherManager.ShowHint($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return;
        }

        var modules = ReadSaveFileModuleList(data);
        if (modules.Length == 0)
            return;

        _launcherManager.ShowFileSave(
            new BUTRTextObject("{=XSxlKweM}Save a Bannerlord Module List File").ToString(),
            $"{saveFile}.bmlist",
            new[]
            {
                    new DialogFileFilter("Bannerlord Module List", new []{ "*.bmlist" }),
                    new DialogFileFilter("Novus Preset", new []{ "*.xml" }),
            },
            fileName =>
            {
                if (string.IsNullOrEmpty(fileName)) return;

                try
                {
                    using var fs = new MemoryStream();
                    switch (Path.GetExtension(fileName))
                    {
                        case ".bmlist":
                            SaveBMList(fs, modules);
                            break;
                        case ".xml":
                            SaveNovusPreset(fs, modules);
                            break;
                    }
                    _launcherManager.WriteFileContent(fileName, fs.ToArray());
                }
                catch (Exception e)
                {
                    _launcherManager.ShowHint($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\nException:\n{e}");
                }
                _launcherManager.ShowHint(new BUTRTextObject("{=VwFQTk5z}Successfully exported list!"));
            }
        );
    }
}