using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Extractor.Config;

namespace Extractor;

public static class PatchScanner
{
    public const string PreStage = "Pre";
    public const string PostStage = "Post";

    public static List<string> Scan(ProjectConfig config, string stage)
    {
        var dir = Path.Join(config.ProjectDir, "Patches", stage);
        var list = Directory.GetFiles(dir).ToList();

        list.Sort();

        return list;
    }

    public static List<string> ScanPre(ProjectConfig config) => Scan(config, PreStage);
    public static List<string> ScanPost(ProjectConfig config) => Scan(config, PostStage);
}
