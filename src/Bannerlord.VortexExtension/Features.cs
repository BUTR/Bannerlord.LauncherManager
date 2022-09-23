using System.Collections.Generic;

namespace Bannerlord.VortexExtension
{
    public static class Features
    {
        public static string InterceptorId => "BUTRLoader.BUTRLoadingInterceptor";
        public static string AssemblyResolverId => "BUTRLoader.BUTRAssemblyResolver";

        public static HashSet<string> FeaturesList = new()
        {
            InterceptorId,
            AssemblyResolverId
        };
    }
}