using System.IO;
using System.Threading.Tasks;
using Extractor.Config;
using SlnEditor.Models;

namespace Extractor.Steps;

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

        /*

    GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|AnyCPU = Debug|AnyCPU
		Release|AnyCPU = Release|AnyCPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{573800AC-9724-48A4-BA8B-0A1A8C914688}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{573800AC-9724-48A4-BA8B-0A1A8C914688}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{573800AC-9724-48A4-BA8B-0A1A8C914688}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{573800AC-9724-48A4-BA8B-0A1A8C914688}.Release|AnyCPU.Build.0 = Release|AnyCPU
		{CA3F565F-9779-49C0-B37E-4FDCAB412C03}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{CA3F565F-9779-49C0-B37E-4FDCAB412C03}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{CA3F565F-9779-49C0-B37E-4FDCAB412C03}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{CA3F565F-9779-49C0-B37E-4FDCAB412C03}.Release|AnyCPU.Build.0 = Release|AnyCPU
		{ABE35572-8BD2-489A-91AA-70530AC87432}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{ABE35572-8BD2-489A-91AA-70530AC87432}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{ABE35572-8BD2-489A-91AA-70530AC87432}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{ABE35572-8BD2-489A-91AA-70530AC87432}.Release|AnyCPU.Build.0 = Release|AnyCPU
		{F8B4D6A1-E6C5-429E-B8DE-30DF7404A2BD}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{F8B4D6A1-E6C5-429E-B8DE-30DF7404A2BD}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{F8B4D6A1-E6C5-429E-B8DE-30DF7404A2BD}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{F8B4D6A1-E6C5-429E-B8DE-30DF7404A2BD}.Release|AnyCPU.Build.0 = Release|AnyCPU
		{9E4B6260-74A2-434C-93FF-EAEF81453DBA}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{9E4B6260-74A2-434C-93FF-EAEF81453DBA}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{9E4B6260-74A2-434C-93FF-EAEF81453DBA}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{9E4B6260-74A2-434C-93FF-EAEF81453DBA}.Release|AnyCPU.Build.0 = Release|AnyCPU
		{7E807690-1F0F-4B2D-9E22-4E6993328EAA}.Debug|AnyCPU.ActiveCfg = Debug|AnyCPU
		{7E807690-1F0F-4B2D-9E22-4E6993328EAA}.Debug|AnyCPU.Build.0 = Debug|AnyCPU
		{7E807690-1F0F-4B2D-9E22-4E6993328EAA}.Release|AnyCPU.ActiveCfg = Release|AnyCPU
		{7E807690-1F0F-4B2D-9E22-4E6993328EAA}.Release|AnyCPU.Build.0 = Release|AnyCPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection

        */

        await File.WriteAllTextAsync(slnPath, sln.ToString());
    }
}
