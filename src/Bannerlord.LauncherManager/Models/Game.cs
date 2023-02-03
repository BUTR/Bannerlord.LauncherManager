/*
using System.Collections.Generic;

namespace Bannerlord.VortexExtension.Models
{
    public record Game : Tool
    {
        ModType[]? modTypes;

        Tool[]? supportedTools;

        string? extensionPath;

        bool mergeMods;

        Dictionary<string, object>? details;

        Dictionary<string, bool>? compatible;

        string? contributed;

        bool? final;

        string? version;

        bool? requiresCleanup;

        DirectoryCleaningMode? directoryCleaning;

        string[]? Overrides;
    }

    public record RequiresLauncherReturn
    {
        public string Launcher { get; set; }
        public object AddInfo { get; set; }
    }

    public record Mod
    {

    }

    public record DiscoveryResult
    {

    }

    public record ModType {
        string typeId;
        int priority;
        //isSupported: (gameId: string) => boolean;
        //getPath: (game: IGame) => string;
        //test: (installInstructions: IInstruction[]) => Promise<boolean>;
        //IModTypeOptions options;
    }

    public enum DirectoryCleaningMode
    {
        tag,
        all
    }
}
*/