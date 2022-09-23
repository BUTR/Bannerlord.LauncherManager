using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Bannerlord.ModuleManager;
using Bannerlord.VortexExtension.Models;

namespace Bannerlord.VortexExtension
{
    public partial class VortexExtensionHandler
    {
        /// <summary>
        /// Checks the mods content and verifies if it is a valid bannerlord mod
        /// </summary>
        public SupportedResult TestModuleContent(string[] files, string gameId)
        {
            if (!string.Equals(gameId, Constants.GameID, StringComparison.OrdinalIgnoreCase))
                return SupportedResult.AsNotSupported;

            if (!files.Any(x => x.Contains(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase)))
                return SupportedResult.AsNotSupported;

            return SupportedResult.AsSupported;
        }

        /// <summary>
        /// Two ways to handle installation:
        /// 1. The root folder is the game's root folder.
        ///     We can install mods to both /Modules and /bin folders
        /// 2. The root folder is the game's /Modules folder
        ///     The /bin mods won't be handled by us and instead they will be
        ///     copied by a fallback mechanism. Vortex will just copy the
        ///     content as is  to the game's root folder
        /// </summary>
        public InstallResult InstallModuleContent(string[] files, string destinationPath)
        {
            if (!files.Any(x => x.Contains(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase)))
                return InstallResult.AsInvalid;

            files = files.Where(x => !Path.EndsInDirectorySeparator(x)).ToArray();

            var moduleIds = new List<string>();
            var instructions = files.Where(x => x.EndsWith(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase)).Select(file =>
            {
                var path = Path.Combine(destinationPath, file);
                var xml = ReadFileContent(path);
                var doc = new XmlDocument();
                doc.LoadXml(xml.ToString());
                var module = ModuleInfoExtended.FromXml(doc);

                var subModuleFileIdx = file.IndexOf(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase);
                var moduleBasePath = file.Substring(0, subModuleFileIdx);
                return files.Where(y => y.StartsWith(moduleBasePath)).Select(file2 => new InstallInstruction
                {
                    Type = InstructionType.copy,
                    Source = file2,
                    Destination = Path.Combine(Constants.ModulesFolder, module.Id, file2.Substring(subModuleFileIdx))
                }).ToList();
            }).SelectMany(x => x).ToList();

            if (instructions.Count > 0)
            {
                instructions.Add(new InstallInstruction
                {
                    Type = InstructionType.attribute,
                    Key = "subModsIds",
                    Value = moduleIds
                });
            }

            return new InstallResult { Instructions = instructions };
        }
    }
}