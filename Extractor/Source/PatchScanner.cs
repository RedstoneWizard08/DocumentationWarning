using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Extractor;

public static class PatchScanner {
    public const string PreStage = "Pre";
    public const string PostStage = "Post";

    public static async Task<List<string>> Scan(string stage) {
        var asm = Assembly.GetExecutingAssembly();
        var resources = asm.GetManifestResourceNames();
        var res = new List<string>();

        foreach (var name in resources) {
            if (name.EndsWith(".patch")) {
                var file = name.Split("Patches.")[1];
                var fileStage = file.Split(".")[0];

                if (fileStage == stage) {
                    var stream = asm.GetManifestResourceStream(name)!;
                    var reader = new StreamReader(stream);
                    var text = await reader.ReadToEndAsync();

                    res.Add(text);
                }
            }
        }

        return res;
    }

    public static Task<List<string>> ScanPre() => Scan(PreStage);
    public static Task<List<string>> ScanPost() => Scan(PostStage); 
}
