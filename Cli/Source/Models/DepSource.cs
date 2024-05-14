using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DocumentationWarning.Models;

public enum DepSource
{
    [JsonProperty("NuGet")]
    NuGet,

    [JsonProperty("BaGet")]
    BaGet,
}

public static class DepSourceExt
{
    public static DepSource Parse(this string str) {
        return str switch {
            "NuGet" => DepSource.NuGet,
            "BaGet" => DepSource.BaGet,
            _ => throw new ArgumentException($"Unknown source: {str}"),
        };
    }

    public static string GetPackageUrl(this DepSource source, string package, string version)
    {
        return source switch
        {
            DepSource.BaGet => $"https://nuget.bepinex.dev/v3/package/{package.ToLower()}/{version}/{package.ToLower()}.{version}.nupkg",
            DepSource.NuGet => $"https://www.nuget.org/api/v2/package/{package.ToLower()}/{version}",
            _ => throw new UnreachableException(),
        };
    }

    public static string GetManifestUrl(this DepSource source, string package)
    {
        return source switch
        {
            DepSource.NuGet => $"https://api.nuget.org/v3-flatcontainer/{package.ToLower()}/index.json",
            DepSource.BaGet => $"https://nuget.bepinex.dev/v3/package/{package.ToLower()}/index.json",
            _ => throw new UnreachableException(),
        };
    }
}
