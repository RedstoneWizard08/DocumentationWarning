using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Util;
using Newtonsoft.Json;

namespace DocumentationWarning.Config;

public class ProjectConfig {
    public required GameConfig Game;
    public required UrlConfig Urls;
    public required string Framework;
    public required List<string> Assemblies = [];
    public required List<Dependency> Dependencies = [];

    [JsonIgnore] public string ProjectDir = "";
    [JsonProperty("tempDir")] public string TempDirStr = "Temp";
    [JsonProperty("outDir")] public string OutDirStr = "Out";

    [JsonIgnore] public string TempDir => Path.Join(ProjectDir, TempDirStr);
    [JsonIgnore] public string OutDir => Path.Join(ProjectDir, OutDirStr);
    [JsonIgnore] public string AsmDir => Path.Join(TempDir, "Assemblies");
    [JsonIgnore] public string PkgDir => Path.Join(TempDir, "Packages");
    [JsonIgnore] public string DocDir => Path.Join(ProjectDir, "Docs");
    [JsonIgnore] public string? DownloadOutputDir;

    public static async Task<ProjectConfig> Load(string dir, string path) {
        var data = await File.ReadAllTextAsync(path);
        var me = JsonConvert.DeserializeObject<ProjectConfig>(data)!;

        if (me.Game.Package != null) {
            var versions = await Http.GetManifest(me.Game.Package, me.Game.Source);
            var latest = versions.versions[^1];

            me.Dependencies.Add(
                new Dependency {
                    Name = me.Game.Package,
                    Version = latest,
                    Source = me.Game.Source,
                }
            );
        }

        if (me.ProjectDir.Length == 0) {
            me.ProjectDir = dir;
        }

        return me;
    }
}