namespace DocumentationWarning.Config;

public class SteamGameConfig {
    public required uint AppId;
    public required uint DepotId;
    public required string AssemblyFolder;

    public string? Branch;
    public string? Os;
    public string? Arch;
    public string? Language;
}