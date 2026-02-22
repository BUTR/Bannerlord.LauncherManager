using Microsoft.Extensions.Logging;

using MLVScan;
using MLVScan.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Native.Utils;

internal class ModScanner
{
    private readonly AssemblyScanner2 _scanner;

    public ModScanner(string folderPath)
    {
        // Create scanner with default rules
        var rules = RuleFactory.CreateDefaultRules();
        var config = new ScanConfig()
        {
            EnableAutoScan = false,
            EnableAutoDisable = false,
            DumpFullIlReports = false,
            DeveloperMode = true,
        };

        _scanner = new AssemblyScanner2(rules, config);
    }

    private static readonly Dictionary<int, List<ScanFinding>> _hashCache = new();

    public List<ScanFinding> ScanMod(byte[] data, string path)
    {
        var hash = data.GetHashCode();
        if (_hashCache.TryGetValue(hash, out var cachedFinding))
            return cachedFinding;

        var findings = ScanModFromBytes(data, path);
        _hashCache[hash] = findings;

        return findings;
    }

    /// <summary>
    /// Scans a mod from a byte array (useful if you already have the assembly loaded in memory).
    /// </summary>
    public List<ScanFinding> ScanModFromBytes(byte[] assemblyBytes, string fileName)
    {
        try
        {
            using var stream = new MemoryStream(assemblyBytes);
            return _scanner.Scan(stream, fileName).ToList();
        }
        catch (Exception ex)
        {
            Logger.ExternalLog(LogLevel.Error, $"Error scanning mod {fileName}, Exception: {ex}");
            return [];
        }
    }
}