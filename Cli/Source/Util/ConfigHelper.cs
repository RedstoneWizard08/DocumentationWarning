using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using Newtonsoft.Json;

namespace DocumentationWarning.Util;

public static class ConfigHelper
{
    public static async Task<List<ProjectConfig>> GetConfigs()
    {
        var configs = new List<ProjectConfig>();
        var projectsData = await File.ReadAllTextAsync("projects.json");
        var projects = JsonConvert.DeserializeObject<List<string>>(projectsData)!;

        foreach (var proj in projects)
        {
            configs.Add(await ProjectConfig.Load(proj, Path.Join(proj, "config.json")));
        }

        return configs;
    }
}
