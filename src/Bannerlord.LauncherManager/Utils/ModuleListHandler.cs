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
using System.Threading.Tasks;
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

    private async Task<bool> ShowMissingWarningAsync(IEnumerable<string> missingModuleIds)
    {
        var missing = new BUTRTextObject("{=GtDRbC3m}Missing Modules:{NL}{MODULES}")
            .SetTextVariable("MODULES", "").ToString();
        
        return await _launcherManager.ShowWarningAsync(
            new BUTRTextObject("Import Warning").ToString(),
            missing,
            $"{string.Join("\n", missingModuleIds)}{Environment.NewLine}{Environment.NewLine}{new BUTRTextObject("{=hgew15HH}Continue import?")}"
        );
    }
    private async Task<bool> ShowVersionWarningAsync(IEnumerable<string> mismatchedVersions)
    {
        var mismatched = new BUTRTextObject("{=BuMom4Jt}Mismatched Module Versions:{NL}{MODULEVERSIONS}")
            .SetTextVariable("NL", Environment.NewLine)
            .SetTextVariable("MODULEVERSIONS", string.Join(Environment.NewLine, mismatchedVersions)).ToString();
        
        var split = mismatched.Split('\n');
        
        return await _launcherManager.ShowWarningAsync(
            new BUTRTextObject("Import Warning").ToString(),
            split[0],
            $"{split[1]}{Environment.NewLine}{Environment.NewLine}{new BUTRTextObject("{=hgew15HH}Continue import?")}"
        );
    }
    private async Task<ModuleInfoExtendedWithMetadata[]> ReadImportListAsync(byte[] data)
    {
        static async IAsyncEnumerable<string> ReadAllLinesAsync(TextReader reader)
        {
            while (await reader.ReadLineAsync() is { } line)
            {
                yield return line;
            }
        }
        async IAsyncEnumerable<ModuleListEntry> DeserializeAsync(IAsyncEnumerable<string> content)
        {
            var list = new List<ModuleListEntry>();
            await foreach (var line in content)
            {
                if (line.Split([": "], StringSplitOptions.RemoveEmptyEntries) is { Length: 2 } split)
                {
                    var version = ApplicationVersion.TryParse(split[1], out var versionVar) ? versionVar : ApplicationVersion.Empty;
                    list.Add(new ModuleListEntry(split[0], version));
                }
            }
            var nativeChangeset = list.Find(x => x.Id == Constants.NativeModule)?.Version.ChangeSet is var y and not 0 ? y : await _launcherManager.GetChangesetAsync();
            foreach (var entry in list)
            {
                var version = entry.Version;
                yield return version.ChangeSet == nativeChangeset
                    ? entry with { Version = new ApplicationVersion(version.ApplicationVersionType, version.Major, version.Minor, version.Revision, 0) }
                    : entry;
            }
        }

        var moduleViewModels = (await _launcherManager.GetModuleViewModelsAsync())?.ToArray() ?? [];

        var idDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended).Select(x => x.Id).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (idDuplicates.Count > 0)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=izSm5f85}Duplicate Module Ids:{NL}{MODULEIDS}").SetTextVariable("MODULEIDS", string.Join("\n", idDuplicates))}");
            return [];
        }

        using var stream = new MemoryStream(data);
        using var reader = new StreamReader(stream);
        var importedModules = await DeserializeAsync(ReadAllLinesAsync(reader)).ToArrayAsync();

        async Task<ModuleInfoExtendedWithMetadata[]> ImportListInternalAsync()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (id, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Id == id) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(id, version, launcherModuleVersion));
            }


            ModuleInfoExtendedWithMetadata[] GetResult() => importedModules.Select(x => moduleViewModels.First(y => y.ModuleInfoExtended.Id == x.Id)).Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                if (!await ShowVersionWarningAsync(mismatchedVersions.Select(x => x.ToString())))
                    return [];
            }
            return GetResult();
        }

        var importedModuleIds = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleIds = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).ToHashSet();
        var missingModuleIds = importedModuleIds.Except(currentModuleIds).ToList();
        if (missingModuleIds.Count > 0)
        {
            if (!await ShowVersionWarningAsync(missingModuleIds))
                return [];
        }

        return await ImportListInternalAsync();
    }
    private async Task<ModuleInfoExtendedWithMetadata[]> ReadSaveFileAsync(byte[] data)
    {
        var moduleViewModels = (await _launcherManager.GetModuleViewModelsAsync())?.ToArray() ?? [];

        var nameDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended).Select(x => x.Name).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (nameDuplicates.Count > 0)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=vCwH9226}Duplicate Module Names:{NL}{MODULENAMES}").SetTextVariable("MODULENAMES", string.Join("\n", nameDuplicates))}");
            return [];
        }

        if (await _launcherManager.GetSaveMetadataAsync(string.Empty, data) is not { } metadata)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=epU06HID}Failed to read the save file!")}");
            return [];
        }

        var changeset = metadata.GetChangeSet();
        var importedModules = metadata.GetModules().Select(x =>
        {
            var version = metadata.GetModuleVersion(x);
            if (version.ChangeSet == changeset)
                version = new ApplicationVersion(version.ApplicationVersionType, version.Major, version.Minor, version.Revision, 0);
            return new ModuleListEntry(x, version);
        }).ToArray();

        async Task<ModuleInfoExtendedWithMetadata[]> ImportSaveInternalAsync()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (name, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Name == name) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(name, version, launcherModuleVersion));
            }


            ModuleInfoExtendedWithMetadata[] GetResult() => importedModules.Select(x => moduleViewModels.First(y => y.ModuleInfoExtended.Name == x.Id)).Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                if (!await ShowVersionWarningAsync(mismatchedVersions.Select(x => x.ToString())))
                    return [];
            }

            return GetResult();
        }

        var importedModuleNames = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleNames = moduleViewModels.Select(x => x.ModuleInfoExtended.Name).ToHashSet();
        var missingModuleNames = importedModuleNames.Except(currentModuleNames).ToList();
        if (missingModuleNames.Count > 0)
        {
            if (!await ShowVersionWarningAsync(missingModuleNames.Select(x => x.ToString())))
                return [];
        }

        return await ImportSaveInternalAsync();
    }
    private async Task<ModuleInfoExtendedWithMetadata[]> ReadNovusPresetAsync(byte[] data)
    {
        var moduleViewModels = (await _launcherManager.GetModuleViewModelsAsync())?.ToArray() ?? [];

        var idDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended).Select(x => x.Id).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (idDuplicates.Count > 0)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=izSm5f85}Duplicate Module Ids:{NL}{MODULEIDS}").SetTextVariable("MODULEIDS", string.Join("\n", idDuplicates))}");
            return [];
        }

        var document = ReaderUtils.Read(data);

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

        async Task<ModuleInfoExtendedWithMetadata[]> ImportNovusInternal()
        {
            var mismatchedVersions = new List<ModuleMismatch>();
            foreach (var (id, version, _) in importedModules)
            {
                if (moduleViewModels.FirstOrDefault(x => x.ModuleInfoExtended.Id == id) is not { } moduleVM) continue;

                var launcherModuleVersion = moduleVM.ModuleInfoExtended.Version;
                if (launcherModuleVersion != version)
                    mismatchedVersions.Add(new ModuleMismatch(id, version, launcherModuleVersion));
            }

            ModuleInfoExtendedWithMetadata[] GetResult() => importedModules.Select(x => moduleViewModels.FirstOrDefault(y => y.ModuleInfoExtended.Id == x.Id)).OfType<IModuleViewModel>().Select(x => x.ModuleInfoExtended).ToArray();

            if (mismatchedVersions.Count > 0)
            {
                if (!await ShowVersionWarningAsync(mismatchedVersions.Select(x => x.ToString())))
                    return [];
            }

            return GetResult();
        }

        var importedModuleIds = importedModules.Select(x => x.Id).ToHashSet();
        var currentModuleIds = moduleViewModels.Select(x => x.ModuleInfoExtended.Id).ToHashSet();
        var missingModuleIds = importedModuleIds.Except(currentModuleIds).ToList();
        if (missingModuleIds.Count > 0)
        {
            if (!await ShowVersionWarningAsync(missingModuleIds.Select(x => x.ToString())))
                return [];
        }

        return await ImportNovusInternal();
    }
    
    /// <summary>
    /// External<br/>
    /// </summary>
    public async Task<bool> ImportAsync()
    {
        var fileName = await _launcherManager.ShowFileOpenAsync(
            new BUTRTextObject("{=DKRNkst2}Open a File with a Load Order").ToString(),
            [
                new DialogFileFilter("Bannerlord Module List", ["*.bmlist"]),
                new DialogFileFilter("Bannerlord Save File", ["*.sav"]),
                new DialogFileFilter("Novus Preset", ["*.xml"]),
                new DialogFileFilter("All files", ["*.*"]),
            ]);
        
        if (string.IsNullOrEmpty(fileName) || await _launcherManager.ReadFileContentAsync(fileName, 0, -1) is not { } data)
            return false;

        try
        {
            switch (Path.GetExtension(fileName))
            {
                case ".bmlist":
                {
                    var result = await ReadImportListAsync(data);
                    await ImportInternalAsync(result);
                    return true;
                }
                case ".sav":
                {
                    var result = await ReadSaveFileAsync(data);
                    await ImportInternalAsync(result);
                    return true;
                }
                case ".xml":
                {
                    var result = await ReadNovusPresetAsync(data);
                    await ImportInternalAsync(result);
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\nException:\n{e}");
            return false;
        }
    }

    /*
    /// <summary>
    /// External<br/>
    /// </summary>
    public void Import()
    {
        ReadImportList(data, result =>
        {
            ImportInternal(result);
            onResult(true);
        }); break;
    }
    */
    
    /// <summary>
    /// External<br/>
    /// </summary>
    public async Task<bool> ImportSaveFileAsync(string saveFile)
    {
        if (await _launcherManager.GetSaveFilePathAsync(saveFile) is not { } saveFilePath)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return false;
        }

        if (await _launcherManager.ReadFileContentAsync(saveFilePath, 0, -1) is not { } data)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return false;
        }

        try
        {
            var result = await ReadSaveFileAsync(data);
            await ImportInternalAsync(result);
            return true;
        }
        catch (Exception e)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\nException:\n{e}");
            return false;
        }
    }
    private async Task ImportInternalAsync(ModuleInfoExtendedWithMetadata[] modules)
    {
        if (modules.Length == 0)
            return;

        var loadOrderValidationIssues = LoadOrderChecker.IsLoadOrderCorrect(modules).ToList();
        if (loadOrderValidationIssues.Count != 0)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=HvvA78sZ}Load Order Issues:{NL}{LOADORDERISSUES}").SetTextVariable("LOADORDERISSUES", string.Join("\n", loadOrderValidationIssues))}");
            return;
        }
        
        var moduleIds = modules.Select(x => x.Id).ToHashSet();
        var (result, orderIssues, orderedModules) = await _launcherManager.TryOrderByLoadOrderAsync(modules.Select(x => x.Id), x => moduleIds.Contains(x));
        if (!result)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=HvvA78sZ}Load Order Issues:{NL}{LOADORDERISSUES}").SetTextVariable("LOADORDERISSUES", string.Join("\n", orderIssues))}");
            return;
        }
        await _launcherManager.SetModuleViewModelsAsync(orderedModules);
    }

    
    private async Task<ModuleListEntry[]> ReadSaveFileModuleListAsync(byte[] data)
    {
        var moduleViewModels = (await _launcherManager.GetModuleViewModelsAsync())?.ToArray() ?? [];

        var nameDuplicates = moduleViewModels.Select(x => x.ModuleInfoExtended).Select(x => x.Name).GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (nameDuplicates.Count > 0)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=vCwH9226}Duplicate Module Names:{NL}{MODULENAMES}").SetTextVariable("MODULENAMES", string.Join("\n", nameDuplicates))}");
            return [];
        }

        if (await _launcherManager.GetSaveMetadataAsync(string.Empty, data) is not { } metadata)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=epU06HID}Failed to read the save file!")}");
            return [];
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
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=GtDRbC3m}Missing Modules:{NL}{MODULES}").SetTextVariable("MODULES", string.Join("\n", missingModuleNames))}");
            return [];
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
    public async Task ExportAsync()
    {
        var fileName = await _launcherManager.ShowFileSaveAsync(
            new BUTRTextObject("{=XSxlKweM}Save a Bannerlord Module List File").ToString(),
            "MyList.bmlist",
            [
                new DialogFileFilter("Bannerlord Module List", ["*.bmlist"]),
                new DialogFileFilter("Novus Preset", ["*.xml"]),
            ]);
        
        if (string.IsNullOrEmpty(fileName)) return;

        try
        {
            var moduleViewModels = await _launcherManager.GetModuleViewModelsAsync() ?? [];

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
            await _launcherManager.WriteFileContentAsync(fileName, fs.ToArray());
        }
        catch (Exception e)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\nException:\n{e}");
        }
        await _launcherManager.ShowHintAsync(new BUTRTextObject("{=VwFQTk5z}Successfully exported list!"));
    }
    
    /// <summary>
    /// External<br/>
    /// </summary>
    public async Task ExportSaveFileAsync(string saveFile)
    {
        if (await _launcherManager.GetSaveFilePathAsync(saveFile) is not { } saveFilePath)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return;
        }

        if (await _launcherManager.ReadFileContentAsync(saveFilePath, 0, -1) is not { } data)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=WJnTxf3v}Cancelled Import!")}\n\n{new BUTRTextObject("{=B64DbmWp}Save File not found!")}");
            return;
        }

        var modules = await ReadSaveFileModuleListAsync(data);
        if (modules.Length == 0)
            return;

        var fileName = await _launcherManager.ShowFileSaveAsync(
            new BUTRTextObject("{=XSxlKweM}Save a Bannerlord Module List File").ToString(),
            $"{saveFile}.bmlist",
            [
                new DialogFileFilter("Bannerlord Module List", ["*.bmlist"]),
                new DialogFileFilter("Novus Preset", ["*.xml"]),
            ]);
        
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
            await _launcherManager.WriteFileContentAsync(fileName, fs.ToArray());
        }
        catch (Exception e)
        {
            await _launcherManager.ShowHintAsync($"{new BUTRTextObject("{=BjtJ4Lxw}Cancelled Export!")}\n\nException:\n{e}");
        }
        await _launcherManager.ShowHintAsync(new BUTRTextObject("{=VwFQTk5z}Successfully exported list!"));
    }
}