using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Processors;

namespace DocumentationWarning.Util;

public static class ResExtractor {
    private static Dictionary<string, string> GetItems() {
        var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        var namesOut = new Dictionary<string, string>();

        foreach (var name in names) {
            var realName = name.Split("Source.Template.")[1];
            var split = realName.Split('.').ToList();
            var ext = split[^1];

            split.RemoveAt(split.Count - 1);

            var path = $"{string.Join('/', split)}.{ext}";

            namesOut.Add(name, path);
        }

        return namesOut;
    }

    public static async Task Extract(string outDir, ProjectConfig config) {
        if (Directory.Exists(outDir)) {
            Directory.Delete(outDir, true);
        }

        var root = await ConfigHelper.GetRootConfig();
        var asm = Assembly.GetExecutingAssembly();
        var proc = TemplateProcessor.FromConfig(root, config);

        foreach (var (key, file) in GetItems()) {
            var path = Path.Join(outDir, file);
            var parent = Directory.GetParent(path)!.FullName;

            if (!Directory.Exists(parent)) {
                Directory.CreateDirectory(parent);
            }

            var inStream = asm.GetManifestResourceStream(key)!;
            var reader = new StreamReader(inStream);
            var data = await reader.ReadToEndAsync();
            var content = proc.Process(data);

            // For some reason, the async version
            // causes inconsistency. Why? idfk
            // ReSharper disable once MethodHasAsyncOverload
            File.WriteAllText(path, content);
        }
    }
}