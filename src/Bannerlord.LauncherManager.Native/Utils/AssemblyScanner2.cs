using Microsoft.Extensions.Logging;

using MLVScan.Abstractions;
using MLVScan.Models;
using MLVScan.Services;

using Mono.Cecil;

using System;
using System.Collections.Generic;
using System.IO;

namespace Bannerlord.LauncherManager.Native.Utils;

/// <summary>
/// Core assembly scanner that analyzes .NET assemblies for malicious patterns.
/// Supports both file-based and stream-based scanning.
/// </summary>
public class AssemblyScanner2
{
    private readonly IReadOnlyList<IScanRule> _rules;
    private readonly TypeScanner _typeScanner;
    private readonly MetadataScanner _metadataScanner;
    private readonly DllImportScanner _dllImportScanner;
    private readonly CallGraphBuilder _callGraphBuilder;
    private readonly DataFlowAnalyzer _dataFlowAnalyzer;
    private readonly IAssemblyResolverProvider _resolverProvider;
    private readonly ScanConfig _config;

    /// <summary>
    /// Creates a new AssemblyScanner with the specified rules and configuration.
    /// </summary>
    /// <param name="rules">The scan rules to apply during analysis.</param>
    /// <param name="config">Optional configuration. Uses defaults if not specified.</param>
    /// <param name="resolverProvider">Optional assembly resolver provider for resolving referenced assemblies.</param>
    public AssemblyScanner2(
        IReadOnlyList<IScanRule> rules,
        ScanConfig? config = null,
        IAssemblyResolverProvider? resolverProvider = null)
    {
        _config = config ?? new ScanConfig();
        _resolverProvider = resolverProvider ?? DefaultAssemblyResolverProvider.Instance;
        _rules = rules;

        // Create all services using composition
        var snippetBuilder = new CodeSnippetBuilder();
        var signalTracker = new SignalTracker(_config);
        var stringPatternDetector = new StringPatternDetector();

        // Create call graph builder for finding consolidation
        _callGraphBuilder = new CallGraphBuilder(rules, snippetBuilder);

        // Create data flow analyzer for tracking data movement through operations
        _dataFlowAnalyzer = new DataFlowAnalyzer(rules, snippetBuilder);

        var reflectionDetector = new ReflectionDetector(rules, signalTracker, stringPatternDetector, snippetBuilder);
        var instructionAnalyzer = new InstructionAnalyzer(rules, signalTracker, reflectionDetector, stringPatternDetector, snippetBuilder, _config, _callGraphBuilder);
        var localVariableAnalyzer = new LocalVariableAnalyzer(rules, signalTracker, _config);
        var exceptionHandlerAnalyzer = new ExceptionHandlerAnalyzer(rules, signalTracker, snippetBuilder, _config);
        var methodScanner = new MethodScanner(rules, signalTracker, instructionAnalyzer, snippetBuilder, localVariableAnalyzer, exceptionHandlerAnalyzer, _config);
        var propertyEventScanner = new PropertyEventScanner(methodScanner, _config);

        _typeScanner = new TypeScanner(methodScanner, signalTracker, reflectionDetector, snippetBuilder, propertyEventScanner, rules, _config);
        _metadataScanner = new MetadataScanner(rules);
        _dllImportScanner = new DllImportScanner(rules, _callGraphBuilder);
    }

    /// <summary>
    /// Scans an assembly from a stream.
    /// Suitable for web scenarios where the assembly is uploaded as a file.
    /// </summary>
    /// <param name="assemblyStream">A readable stream containing the assembly bytes.</param>
    /// <param name="virtualPath">Optional virtual path for error messages.</param>
    /// <returns>A collection of scan findings.</returns>
    /// <exception cref="ArgumentException">Thrown when the stream is null or not readable.</exception>
    public IEnumerable<ScanFinding> Scan(Stream assemblyStream, string? virtualPath = null)
    {
        if (assemblyStream is not { CanRead: true })
            throw new ArgumentException("Assembly stream must be readable", nameof(assemblyStream));

        // Ensure stream is at the beginning
        if (assemblyStream.CanSeek)
            assemblyStream.Position = 0;

        var findings = new List<ScanFinding>();

        try
        {
            // Clear builders for fresh scan
            _callGraphBuilder.Clear();
            _dataFlowAnalyzer.Clear();

            var readerParameters = new ReaderParameters
            {
                ReadWrite = false,
                InMemory = true,
                ReadSymbols = false,
                AssemblyResolver = _resolverProvider.CreateResolver(),
            };

            var assembly = AssemblyDefinition.ReadAssembly(assemblyStream, readerParameters);
            ScanAssembly(assembly, findings);

            // Build consolidated call chain findings
            var callChainFindings = _callGraphBuilder.BuildCallChainFindings();
            findings.AddRange(callChainFindings);

            // Build data flow findings
            var dataFlowFindings = _dataFlowAnalyzer.BuildDataFlowFindings();
            findings.AddRange(dataFlowFindings);
        }
        catch (Exception ex)
        {
            Logger.ExternalLog(LogLevel.Error, ex.ToString());
            findings.Add(new ScanFinding(
                virtualPath ?? "Assembly scanning",
                "Warning: Some parts of the assembly could not be scanned. Please ensure this is a valid Unity mod. This doesn't necessarily mean the mod is malicious."));
        }

        return FilterEmptyFindings(findings);
    }

    private void ScanAssembly(AssemblyDefinition assembly, List<ScanFinding> findings)
    {
        foreach (var module in assembly.Modules)
        {
            // Scan assembly metadata for hidden payloads
            if (_config.DetectAssemblyMetadata)
            {
                findings.AddRange(_metadataScanner.ScanAssemblyMetadata(assembly));
            }

            // Scan for P/Invoke declarations - these are registered with CallGraphBuilder
            // and findings are generated later in BuildCallChainFindings()
            _dllImportScanner.ScanForDllImports(module);

            foreach (var type in module.Types)
            {
                findings.AddRange(_typeScanner.ScanType(type));

                // Phase 1: Analyze data flow for each method in the type (single-method analysis)
                foreach (var method in type.Methods)
                {
                    _dataFlowAnalyzer.AnalyzeMethod(method);
                }
            }
        }

        // Phase 2: Analyze cross-method data flows after all methods have been processed
        _dataFlowAnalyzer.AnalyzeCrossMethodFlows();

        // Phase 3: Allow rules to refine findings using post-analysis information
        // (e.g., recursive embedded resource scanning, DataFlowAnalyzer chain correlation)
        foreach (var module in assembly.Modules)
        {
            foreach (var rule in _rules)
            {
                var refinedFindings = rule.PostAnalysisRefine(module, findings);
                findings.AddRange(refinedFindings);
            }
        }
    }

    private static IEnumerable<ScanFinding> FilterEmptyFindings(List<ScanFinding> findings)
    {
        // Filter out single low-severity warning when nothing else was found
        if (findings is [{ Location: "Assembly scanning" }] && string.IsNullOrEmpty(findings[0].CodeSnippet))
        {
            return new List<ScanFinding>();
        }

        return findings;
    }
}