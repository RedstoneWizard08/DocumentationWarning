using System.Diagnostics;

namespace Extractor;

public enum DepSource
{
    NuGet,
    BaGet,
}

public static class DepSourceExt
{
    public static string GetPackageUrl(this DepSource source, string package, string version)
    {
        return source switch
        {
            DepSource.BaGet => $"https://nuget.bepinex.dev/v3/package/{package.ToLower()}/{version}/{package.ToLower()}.{version}.nupkg",
            DepSource.NuGet => $"https://www.nuget.org/api/v2/package/{package.ToLower()}/{version}",
            _ => throw new UnreachableException(),
        };
    }
}
