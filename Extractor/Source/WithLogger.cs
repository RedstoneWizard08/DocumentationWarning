using Microsoft.Extensions.Logging;

namespace Extractor;

public class WithLogger {
    internal readonly ILogger Logger = null!;

    protected WithLogger() {
        Logger = Program.Factory.CreateLogger(GetType().FullName!);
    }
}
