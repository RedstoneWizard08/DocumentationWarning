using System.IO;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using SlnEditor.Models;

namespace DocumentationWarning.Steps;

public sealed class GenSolution : Step
{
    public override async Task Run(ProjectConfig config)
    {
        var slnPath = Path.Join(config.OutDir, "Project.sln");
        var sln = new Solution();

        sln.ConfigurationPlatforms.Add(new ConfigurationPlatform("Debug|AnyCPU", "Debug", "AnyCPU"));
        sln.ConfigurationPlatforms.Add(new ConfigurationPlatform("Release|AnyCPU", "Release", "AnyCPU"));
        sln.SolutionProperties.HideSolutionNode = false;

        foreach (var assembly in config.Assemblies)
        {
            var name = Path.GetFileNameWithoutExtension(assembly);
            var proj = new Project(name, Path.Join(name, name + ".csproj"), ProjectType.CSharp);

            proj.ConfigurationPlatforms.Add(new ConfigurationPlatform("Debug|AnyCPU.ActiveCfg", "Debug", "AnyCPU"));
            proj.ConfigurationPlatforms.Add(new ConfigurationPlatform("Debug|AnyCPU.Build.0", "Debug", "AnyCPU"));
            proj.ConfigurationPlatforms.Add(new ConfigurationPlatform("Release|AnyCPU.ActiveCfg", "Release", "AnyCPU"));
            proj.ConfigurationPlatforms.Add(new ConfigurationPlatform("Release|AnyCPU.Build.0", "Release", "AnyCPU"));

            sln.RootProjects.Add(proj);
        }

        await File.WriteAllTextAsync(slnPath, sln.ToString());
    }
}
