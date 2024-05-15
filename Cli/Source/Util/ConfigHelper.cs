using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using Newtonsoft.Json;

namespace DocumentationWarning.Util;

public class ConfigHelper : WithLogger
{
    public static async Task<RootConfig> GetRootConfig() {
        if (!File.Exists("root.json"))
        {
            "Cannot find a root.json! Maybe you need to create it?".LogCritical(new ConfigHelper());

            Environment.Exit(1);
        }

        var projectsData = await File.ReadAllTextAsync("root.json");
        var config = JsonConvert.DeserializeObject<RootConfig>(projectsData)!;

        return config;
    }

    public static async Task<List<ProjectConfig>> GetConfigs()
    {
        var configs = new List<ProjectConfig>();
        var config = await GetRootConfig();

        foreach (var proj in config.Projects)
        {
            configs.Add(await ProjectConfig.Load(proj, Path.Join(proj, "config.json")));
        }

        return configs;
    }
}
