﻿using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bannerlord.LauncherManager
{
    public partial class LauncherManagerHandler
    {
        private readonly IModulePathProvider[] _providers;

        public LauncherManagerHandler()
        {
            _providers = new IModulePathProvider[]
            {
                new MainModuleProvider(this),
                new SteamModuleProvider(this)
            };
        }

        public IEnumerable<ModuleInfoExtended> GetModules()
        {
            foreach (var modulePath in GetModulePaths())
            {
                var subModulePath = Path.Combine(modulePath, Constants.SubModuleName);
                if (ReadFileContent(subModulePath) is { } content)
                {
                    ModuleInfoExtended? moduleInfoExtended;
                    try
                    {
                        var xml = new XmlDocument();
                        xml.LoadXml(content);
                        moduleInfoExtended = ModuleInfoExtended.FromXml(xml);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"modulePath: {modulePath}, content: {content}", e);
                    }
                    if (moduleInfoExtended is not null)
                        yield return moduleInfoExtended;
                }
            }
        }

        public IEnumerable<string> GetModulePaths()
        {
            foreach (var provider in _providers)
            {
                foreach (var modulePath in provider.GetModulePaths())
                {
                    yield return modulePath;
                }
            }
        }
    }
}