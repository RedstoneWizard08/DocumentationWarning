using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentationWarning.Config;

namespace DocumentationWarning.Util;

public static class PatchScanner
{
    public const string PreStage = "Pre";
    public const string PostStage = "Post";

    public static List<string> Scan(ProjectConfig config, string stage)
    {
        var dir = Path.Join(config.ProjectDir, "Patches", stage);
        if (!Directory.Exists(dir)) return [];
        var list = Directory.GetFiles(dir).Where(v => v.EndsWith(".patch")).ToList();

        list.Sort();

        return list;
    }

    public static List<string> ScanPre(ProjectConfig config) => Scan(config, PreStage);
    public static List<string> ScanPost(ProjectConfig config) => Scan(config, PostStage);
}
