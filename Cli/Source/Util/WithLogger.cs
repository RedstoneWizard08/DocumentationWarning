using DocumentationWarning.Logging;

namespace DocumentationWarning.Util;

public class WithLogger
{
    internal readonly Logger Logger = null!;

    protected WithLogger()
    {
        Logger = new();
    }
}
