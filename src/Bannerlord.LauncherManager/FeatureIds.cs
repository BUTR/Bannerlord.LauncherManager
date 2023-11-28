using System.Collections.Generic;

namespace Bannerlord.LauncherManager;

public static class FeatureIds
{
    public static string InterceptorId => "BLSE.LoadingInterceptor";
    public static string AssemblyResolverId => "BLSE.AssemblyResolver";
    private static string InterceptorIdOld => "BUTRLoader.BUTRLoadingInterceptor";
    private static string AssemblyResolverIdOld => "BUTRLoader.BUTRAssemblyResolver";
    public static string ContinueSaveFileId => "BLSE.ContinueSaveFile";
    public static string CommandsId => "BLSE.Commands";

    public static readonly HashSet<string> Features = new()
    {
        InterceptorId,
        InterceptorIdOld,
        AssemblyResolverId,
        AssemblyResolverIdOld,
        ContinueSaveFileId,
        CommandsId,
    };
    public static readonly HashSet<string> LauncherFeatures = new()
    {
        InterceptorId,
        InterceptorIdOld,
        AssemblyResolverId,
        AssemblyResolverIdOld,
    };
}