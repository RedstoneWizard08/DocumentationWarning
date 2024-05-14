using System.IO;

namespace Extractor.Config;

public class GameConfig
{
    public required string Id;
    public required string Name;
    public required string Publisher;
    public required string Package;
    public required DepSource Source;
    public required string Namespace;
    public required string Unity;
    public required string Banner;
    public required string Icon;

    public string BannerPath(ProjectConfig config) => Banner.StartsWith('/') ? Banner : Path.Join(config.ProjectDir, Banner);
    public string IconPath(ProjectConfig config) => Icon.StartsWith('/') ? Icon : Path.Join(config.ProjectDir, Icon);
}
