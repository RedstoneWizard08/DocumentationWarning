using DocumentationWarning.Models;

namespace DocumentationWarning.Config;

public class Dependency
{
    public required string Name;
    public required string Version;
    public required DepSource Source;
}
