using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Extractor;

public static class Config
{
    public const string Package = "ContentWarning.GameLibs.Steam";
    public const string Framework = "netstandard2.1";
    public const string UnityFramework = "netstandard2.0";
    public const string FinalPatchFile = "SP-2024-05-12-FixNamespace.patch";

    public const string TempDir = "Temp";
    public const string OutDir = "Out";

    public static readonly string[] RemovedDirs = ["Photon", "UnityEngine", "UnityEditor", "Properties"];

    public static readonly string AsmDir = Path.Join(TempDir, "Assemblies");
    public static readonly string PkgDir = Path.Join(TempDir, "Packages");
    
    // TODO: Scanning?
    public static readonly string[] Patches = [
        "0000-2024-05-10-FixBase.patch",
        "0001-2024-05-11-FixMissing.patch",
        "0002-2024-05-12-FixRefs.patch",
    ];

    public static List<(string, string, DepSource)> Deps = [
        ("UnityEngine.Modules", "2022.3.10", DepSource.BaGet),
        ("WebSocketSharp", "1.0.3-rc11", DepSource.NuGet),
        ("Newtonsoft.JSON", "13.0.3", DepSource.NuGet),
    ];

    public static readonly string[] Assemblies = [
        "Assembly-CSharp.dll",
        "Zorro.Core.Runtime.dll",
        "Zorro.UI.Runtime.dll",
        "Zorro.Recorder.dll",
        "Zorro.Settings.Runtime.dll",
        "Zorro.PhotonUtility.dll",
    ];

    static Config()
    {
        var task = Http.GetManifest(Package);
        var versions = task.GetAwaiter().GetResult();
        var latest = versions.versions.Last();

        Deps.Add((Package, latest, DepSource.NuGet));
    }
}
